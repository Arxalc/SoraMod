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

public class EnduringFaith() : SoraModCard(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    
    protected override bool HasEnergyCostX => true;
    
    protected override bool ShouldGlowGoldInternal
    {
        get => this.Owner.PlayerCombatState.Energy >= this.DynamicVars.Energy.IntValue;
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12M, ValueProp.Move), (DynamicVar) new EnergyVar(4)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull((object) play.Target, "cardPlay.Target");
        int hitCount = this.ResolveEnergyXValue();
        if (hitCount >= this.DynamicVars.Energy.IntValue)
            hitCount *= 2;
        AttackCommand attackCommand = await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).WithHitCount(hitCount).FromCard((CardModel) this, play).Targeting(play.Target).WithHitFx("vfx/vfx_giant_horizontal_slash", tmpSfx: "slash_attack.mp3").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(3M);
    }
}