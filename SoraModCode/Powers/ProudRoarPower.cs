using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Powers;

// What if I made a blanket Charging/Focusing power for Sea Spectacle too that tracks cards played, damage taken, etc...?
public class ProudRoarPower() : SoraModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced; // ...does this work?
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20M, ValueProp.Unpowered)]; // unpowered...

    
    private bool playerTookDamage = false;
    

    public void SetDamage(Decimal damage)
    {
        this.AssertMutable();
        this.DynamicVars.Damage.BaseValue = damage;
    }
    
    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target == Owner && result.UnblockedDamage > 0)
        {
            playerTookDamage = true;
            // Look into power icon animations for this getting removed
            PowerCmd.Remove(this);
        }
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player && playerTookDamage == false)
        {
            IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, (IEnumerable<Creature>) this.CombatState.HittableEnemies, this.DynamicVars.Damage, this.Owner);
        }
        PowerCmd.Remove(this); // what if there are multiple?
    }


}