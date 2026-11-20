using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class GoofyTornado() : SoraModCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8M, ValueProp.Move), new DynamicVar("BlockValue", 4M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<KnockSmash>(this.IsUpgraded), HoverTipFactory.FromCard<Bombardier>(this.IsUpgraded), HoverTipFactory.FromCard<Trinity>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull((object) play.Target!, "cardPlay.Target");
        Decimal num = await CreatureCmd.GainBlock(this.Owner.Creature,this.DynamicVars["BlockValue"].BaseValue, ValueProp.Unpowered | ValueProp.Move, play);
        AttackCommand attackCommand = await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard((CardModel) this, play).Targeting(play.Target).WithHitFx("vfx/vfx_flying_slash").Execute(choiceContext);
        
        CardModel createdCard = CombatState.CreateCard<KnockSmash>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(createdCard);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2M);
        DynamicVars["BlockValue"].UpgradeValueBy(1);
    }
}