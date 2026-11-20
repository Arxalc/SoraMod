using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards;

public class AntiForm() : SoraModCard(1, CardType.Curse, CardRarity.Basic, TargetType.Self), ITranscendenceCard
{
    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<RageForm>();
    
    public override bool CanBeGeneratedByModifiers => false;
    
    public override bool GainsBlock => IsUpgraded;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5M, ValueProp.Move), new PowerVar<AntiFormPower>(2M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Eternal];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AntiFormPower>()];
    
    private bool _isFreeByThis = false; // To account for other effects that might make this free.

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        
    }
    
    // Might have to do Gain this power at the end of the turn...
    // 'cuz this counter may just end at the end of the turn it was applied.

    protected override void OnUpgrade()
    {
        // Gain 5 block on turn end.
    }
    
    public override bool HasTurnEndInHandEffect => true;
    
    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        if (IsUpgraded)
        {
            Decimal num = await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, null);
        }
        AntiFormPower? antiFormPower = await PowerCmd.Apply<AntiFormPower>(choiceContext, this.Owner.Creature, DynamicVars["AntiFormPower"].BaseValue, this.Owner.Creature, (SoraModCard) this);
        // TODO: Double check the counter amount if Anti form is blocked in successive turns
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