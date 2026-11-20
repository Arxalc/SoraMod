using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Powers;

public class OnceMorePower() : SoraModPower
{
    // Similar to Lizard Tail relic
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private bool _hasTriggered = false;

    // ✅ ShouldDieLate prevents death when triggered
    public override bool ShouldDieLate(Creature creature)
    {
        // Only apply to the owner of THIS power instance
        if (creature != this.Owner) return false;
        
        CreatureCmd.SetCurrentHp(this.Owner, 1);
        this.Flash();
        _hasTriggered = true;
        return false;
    }
    
    public override async Task AfterPreventingDeath(Creature creature)
    {
        await CreatureCmd.SetCurrentHp(this.Owner, 1);
    }
    
    // Removes at the end of ENEMY turn
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy)
            return;
        
        if (_hasTriggered)
            await PowerCmd.Remove((PowerModel) this);
    }
    
}