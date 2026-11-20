using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Powers;

/**
 * Replays first played attack card
 */
public class DualWieldPower() : SoraModPower
{
    private class Data
    {
        public bool hasReplayedThisTurn = false;
    }
    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single; // Hmm maybe make this a Counter and then do more echo form-like logic

    // Mimicking Echo Form...
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        // Check for valid cards
        if (card.Owner.Creature != this.Owner || card.Type != CardType.Attack)
        {
            return playCount;
        }
        
        int numAttacksPlayedThisTurn = CombatManager.Instance.History.CardPlaysStarted.Count(e => e.Actor == this.Owner && e.CardPlay.Card.Type == CardType.Attack && e.HappenedThisTurn(this.CombatState));
        
        if (numAttacksPlayedThisTurn == 0)
        {
            return playCount + 1;
            /*
            // Optional: Check that we haven't already used this effect this turn
            // (Only needed if you want it to work once per turn) which Echo Form may and when I want o make this into a Counter.
            var data = this.GetInternalData<Data>();
            if (!data.hasReplayedThisTurn)
            {
                data.hasReplayedThisTurn = true;
                return playCount + 1;  // Replay the card
            }
            */
        }
        return playCount;
    }
}