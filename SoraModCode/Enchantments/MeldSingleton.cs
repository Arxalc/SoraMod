using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Runs;

namespace SoraMod.SoraModCode.Enchantments;

public class MeldSingleton() : CustomSingletonModel(HookType.Combat)
{
    /*
     * Copies the deck's saved melded card info to the combat copy's info.
     */
    public override async Task BeforeCombatStart()
        {
            var combatState = CombatManager.Instance.DebugOnlyGetState();
            if (combatState == null)
                return;

            foreach (var creature in combatState.PlayerCreatures)
            {
                var player = creature.Player;
                if (player == null) continue;

                foreach (var combatCard in PileType.Draw.GetPile(player).Cards)
                {
                    if (combatCard.Enchantment is not Meld combatMeld) continue;

                    var matching = PileType.Deck.GetPile(player).Cards
                        .Where(c => c.Id == combatCard.Id && c.IsUpgraded == combatCard.IsUpgraded)
                        .ToList();

                    // Sync from the first matching deck card that actually has a Meld with a target
                    var source = matching.FirstOrDefault(c => c.Enchantment is Meld dm && dm.MeldTargetId != null);
                    if (source?.Enchantment is Meld deckMeld)
                    {
                        combatMeld.MeldTargetId = deckMeld.MeldTargetId;
                        combatMeld.MeldTargetWasUpgraded = deckMeld.MeldTargetWasUpgraded;
                    }
                }
            }
        }
}