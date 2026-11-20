using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class KnockSmash() : SoraModCard(2, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    private int unblockedDamage = 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2M, ValueProp.Move), new RepeatVar(5)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Bombardier>(this.IsUpgraded), HoverTipFactory.FromCard<Trinity>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull((object) play.Target!, "cardPlay.Target");
        AttackCommand attackCommand = await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).WithHitCount(this.DynamicVars.Repeat.IntValue).FromCard((CardModel) this, play).Targeting(play.Target).WithHitFx("vfx/vfx_flying_slash").Execute(choiceContext);
        BlockVar toBlock = new BlockVar(unblockedDamage, ValueProp.Unpowered); // Unpowered
        Decimal num = await CreatureCmd.GainBlock(this.Owner.Creature, toBlock, play); // I think this works by itself.
        unblockedDamage = 0;
        
        CardModel createdCard = CombatState.CreateCard<Bombardier>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(createdCard);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1M);
    }

    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (cardSource == this && target.IsEnemy && result.UnblockedDamage > 0)
        {
            unblockedDamage = unblockedDamage + result.UnblockedDamage;
        }
        return Task.CompletedTask;
    }
}