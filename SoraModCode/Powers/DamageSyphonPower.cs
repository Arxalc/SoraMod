using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Powers;

public class DamageSyphonPower() : SoraModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        // What if you had thorns and the enemy died while hitting you? Should still go off.
        // TODO: Test this!
        if (target == Owner && result.UnblockedDamage > 0 && CombatState.Enemies.Contains(dealer))
        {
            FreeMagicPower strengthPower = await PowerCmd.Apply<FreeMagicPower>(choiceContext, this.Owner, 1, this.Owner, (CardModel) null);
        }
    }
    
}