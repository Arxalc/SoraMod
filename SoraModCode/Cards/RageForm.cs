using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards;

public class RageForm() : SoraModCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    public override bool CanBeGeneratedByModifiers => false;

    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10M, ValueProp.Move), new PowerVar<RageFormPower>(2M), new EnergyVar(1)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Eternal];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RageFormPower>()];

    private bool _isFreeByThis = false; // To account for other effects that might make this free.
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Decimal num = await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5M);
    }
    
    public override bool HasTurnEndInHandEffect => true;
    
    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        if (IsUpgraded)
        {
            Decimal num = await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, null);
        }
        RageFormPower? rageFormPower = await PowerCmd.Apply<RageFormPower>(choiceContext, this.Owner.Creature, DynamicVars["RageFormPower"].BaseValue, this.Owner.Creature, (SoraModCard) this);
        // TODO: Double check the counter amount if Rage form is blocked in successive turns
    }
    
    /* Let's not make this more complicated...
    public override Task AfterCurrentHpChanged(Creature creature, Decimal delta)
    {
        if (Owner.Creature.CurrentHp <= Owner.Creature.MaxHp / 4 && !_isFreeByThis) // < 25% max hp
        {
            SetToFreeThisCombat();
            _isFreeByThis = true;
        }
        else if (delta > 0 && !_isFreeByThis) // was healed
        {
            if (Owner.Creature.CurrentHp > Owner.Creature.MaxHp / 4)
            {
                EnergyCost.SetThisCombat(DynamicVars.Energy.IntValue);
                _isFreeByThis = true;
            }
        }
        return Task.CompletedTask;
    }
    */
}