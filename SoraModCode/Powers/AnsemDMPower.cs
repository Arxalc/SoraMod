using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Encounters;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Powers;

public class AnsemDMPower() : SoraModPower
{
    // PlowPower
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ConfusedPower>()];

    public override bool ShouldScaleInMultiplayer => true; // technically, but should always be 30% of hp.
    
    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {

        if (target != Owner || result.UnblockedDamage <= 0 || target.CurrentHp > Amount)
            return;

        Flash();

        if (Owner.Monster is Ansem ansem)
        {
            await ansem.SetDesperation();
        }
        await PowerCmd.Remove(this);
    }
}