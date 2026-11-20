using System.Diagnostics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Powers;

public class MagnetPower() : SoraModPower
{
    /*
     * There is no good way to directly change a card's target. >-<
     */
    
    // TODO: Make sure this works as intended. I'll come back to this later...
    
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private CardModel playedCard;
    private Dictionary<Type, int> prevPowers = new();
    private Dictionary<Type, int> afterPowers = new();
    private Dictionary<Type, int> diffPowers = new();
    
    private bool _isProcessingSpread = false; // Lock
    
        public override async Task BeforeCardPlayed(CardPlay play)
        {
            var target = play.Target;
            if (target == null) return;
            if (target != this.Owner) return;
            if (play.Card.Type != CardType.Attack) return;

            prevPowers.Clear();
            foreach (var power in target.Powers)
            {
                prevPowers[power.GetType()] = power.Amount;
            }
            
            Log.Info($"MagnetPower: Stored {prevPowers.Count} powers before attack for target {target.GetHashCode()}");
            await Task.CompletedTask;
        }
        
        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (_isProcessingSpread) return;
            
            var card = play.Card;
            var target = play.Target;
            
            // ONLY process if THIS power's owner is the target
            if (target == null) return;
            if (target != this.Owner) return;
            if (card.Type != CardType.Attack) return;

            Log.Info($"MagnetPower: Processing attack card {card.Id} - target is THIS enemy!");

            // Record powers AFTER the card has fully resolved
            afterPowers.Clear();
            foreach (var power in target.Powers)
            {
                afterPowers[power.GetType()] = power.Amount;
                Log.Info($"MagnetPower: AFTER - {power.GetType().Name}: {power.Amount}");
            }

            // Calculate differences
            CalculatePowersDifference();

            // Get all other Magnet enemies
            var allMagnetEnemies = CombatState.HittableEnemies
                .Where(c => c.HasPower<MagnetPower>() && c.IsAlive && c != target)
                .ToList();

            if (allMagnetEnemies.Count == 0) return;

            _isProcessingSpread = true;

            try
            {
                // Get damage and hit count
                Decimal damagePerHit = GetDamagePerHit(card, null);
                int hitCount = GetHitCount(card);

                // Spread to all other Magnet enemies
                foreach (var enemy in allMagnetEnemies)
                {
                    if (!enemy.IsAlive) continue;

                    Log.Info($"MagnetPower: Spreading damage to enemy {enemy.GetHashCode()}");
                    
                    // Spread damage
                    await DamageCmd.Attack(damagePerHit)
                        .WithHitCount(hitCount)
                        .FromCard(card, null)
                        .Targeting(enemy)
                        .Unpowered()
                        .WithNoAttackerAnim()
                        .Execute(choiceContext);
                    
                    // Spread powers
                    foreach (var diff in diffPowers)
                    {
                        Type powerType = diff.Key;
                        int amountToSpread = diff.Value;
                        
                        if (powerType == typeof(MagnetPower)) continue;
                                
                        var sourcePower = target.Powers.FirstOrDefault(p => p.GetType() == powerType);
                        if (sourcePower == null) continue;
                                
                        var clonedPower = sourcePower.ClonePreservingMutability() as PowerModel;
                        if (clonedPower == null) continue;
                                
                        Log.Info($"MagnetPower: Spreading {powerType.Name} x {amountToSpread} to enemy {enemy.GetHashCode()}");
                        
                        await PowerCmd.Apply(
                            choiceContext,
                            clonedPower,
                            enemy,
                            amountToSpread,
                            card.Owner.Creature,
                            card
                        );
                    }
                }
            }
            finally
            {
                diffPowers.Clear();
                _isProcessingSpread = false;
            }
        }

        // ============ CALCULATE DIFFERENCES ============
        private void CalculatePowersDifference()
        {
            diffPowers.Clear();
            
            Log.Info($"MagnetPower: CalculatePowersDifference - prevPowers: {prevPowers.Count}, afterPowers: {afterPowers.Count}");
            
            foreach (var after in afterPowers)
            {
                Type powerType = after.Key;
                int afterAmount = after.Value;
                
                if (prevPowers.TryGetValue(powerType, out int beforeAmount))
                {
                    int difference = afterAmount - beforeAmount;
                    if (difference > 0)
                    {
                        diffPowers[powerType] = difference;
                        Log.Info($"MagnetPower: ✅ {powerType.Name} increased by {difference} (was {beforeAmount}, now {afterAmount})");
                    }
                }
                else
                {
                    diffPowers[powerType] = afterAmount;
                    Log.Info($"MagnetPower: ✅ New power {powerType.Name} with amount {afterAmount}");
                }
            }
            
            Log.Info($"MagnetPower: diffPowers has {diffPowers.Count} entries");
        }
        
        private Decimal GetDamagePerHit(CardModel card, Creature? target)
        {
            if (card.DynamicVars.TryGetValue("Damage", out var damageVar))
            {
                if (damageVar is CalculatedDamageVar calculatedDamage)
                {
                    return calculatedDamage.Calculate(target ?? this.Owner);
                }
                return damageVar.BaseValue;
            }
            return 0;
        }

        private int GetHitCount(CardModel card)
        {
            if (card.DynamicVars.TryGetValue("Repeat", out var repeatVar))
            {
                return (int)repeatVar.BaseValue;
            }
            return 1;
        }
        
        public override async Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (side != CombatSide.Enemy)
                return;
            prevPowers.Clear();
            afterPowers.Clear();
            diffPowers.Clear();
            
            await Task.CompletedTask;
            await PowerCmd.TickDownDuration((PowerModel) this);
        }
}