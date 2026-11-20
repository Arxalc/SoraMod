using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class Explosion() : SoraModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Magic, SoraModKeywords.Finisher];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(6M),
        new ExtraDamageVar(5M),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => 
            (Decimal)CombatManager.Instance.History.CardPlaysStarted
                .Count(e => e.CardPlay.Card.Keywords.Contains(SoraModKeywords.Magic) &&
                            e.HappenedThisTurn(card.CombatState)))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        AttackCommand attackCommand = await DamageCmd.Attack(this.DynamicVars.CalculatedDamage)
            .FromCard(this, play)
            .Targeting(play.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {

    }

    /*
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Card.Owner == Owner && play.Card.Keywords.Contains(SoraModKeywords.Magic))
        {
            
        }
    }
    */

}