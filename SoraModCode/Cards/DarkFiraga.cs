using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class DarkFiraga() : SoraModCard(2, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12M, ValueProp.Move), new PowerVar<VulnerablePower>(2M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromCard<DarkBarrier>(this.IsUpgraded), HoverTipFactory.FromCard<NightmaresEnd>(this.IsUpgraded), HoverTipFactory.FromCard<EternalSession>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull((object) play.Target!, "cardPlay.Target");
        VulnerablePower vulnerablePower = await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, this.DynamicVars.Vulnerable.BaseValue, this.Owner.Creature, (CardModel) this);
        AttackCommand attackCommand = await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard((CardModel) this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        
        CardModel createdCard = CombatState.CreateCard<DarkBarrier>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(createdCard);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Vulnerable.UpgradeValueBy(1M);
    }
}