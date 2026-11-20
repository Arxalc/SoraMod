using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class ArsArcanum() : SoraModCard(5, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // Maybe lower energy cost to 3...
    // Stomp
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3M, ValueProp.Move), new RepeatVar(13)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Finisher];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        AttackCommand attackCommand = await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).WithHitCount(this.DynamicVars.Repeat.IntValue).FromCard((CardModel) this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // Maybe upgrade damage by 1?
        EnergyCost.UpgradeBy(-1);
    }
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || this.IsClone)
            return Task.CompletedTask;
        this.ReduceCostBy(CombatManager.Instance.History.CardPlaysFinished.Count<CardPlayFinishedEntry>((Func<CardPlayFinishedEntry, bool>) (e => e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Player == this.Owner && e.HappenedThisTurn(this.CombatState))));
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != this.Owner || cardPlay.Card.Type != CardType.Attack) // Sure Links can count for this.
            return Task.CompletedTask;
        this.ReduceCostBy(1);
        return Task.CompletedTask;
    }

    private void ReduceCostBy(int amount) => this.EnergyCost.AddThisTurn(-amount);
}