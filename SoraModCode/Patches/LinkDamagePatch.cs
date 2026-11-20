using BaseLib.Cards.Variables;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Keywords;

#nullable enable
namespace BaseLib.Patches.Features;

public static class LinkKeywordPatches
{
    // Can't figure out how to global patch so I'm doing each effect manually...
    // Also need to double check if this is played and it's not the owner... Like you gifted a card... Does Networking count as gifting?
    // Strength, Weakness, Vigor, Gigantification, Double Damage from Shadow Step, Tracking, Lethality, One for all, Shrink
    [HarmonyPatch(typeof(StrengthPower), nameof(StrengthPower.ModifyDamageAdditive), new Type[] { typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay) })]
    public class StrengthPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CardModel? cardSource, Creature? dealer, ref decimal __result)
        {
            if (cardSource?.Keywords.Contains(SoraModKeywords.Link) == true && dealer == cardSource.Owner?.Creature)
            {
                __result = 0M;   
                return false;    // Skip the original method
            }
            return true; // Run the original method if conditions aren't met
        }
    }

    [HarmonyPatch(typeof(WeakPower), nameof(WeakPower.ModifyDamageMultiplicative), new Type[] { typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay) })]
    public class WeakPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CardModel? cardSource, Creature? dealer, ref decimal __result)
        {
            if (cardSource?.Keywords.Contains(SoraModKeywords.Link) == true && dealer == cardSource.Owner?.Creature)
            {
                __result = 1M;   
                return false;    // Skip the original method
            }
            return true; // Run the original method if conditions aren't met
        }
    }

    
    [HarmonyPatch(typeof(VigorPower), nameof(VigorPower.BeforeAttack), new Type[] { typeof(AttackCommand)})]
    public class VigorBeforeAttackPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(AttackCommand command, ref Task __result)
        {
            if (command.CardPlay?.Card.Keywords.Contains(SoraModKeywords.Link) == true)
            {
                // Skip the original method
                __result = Task.CompletedTask;
                return false;
            }
        
            // Run the original method normally for non-Link cards
            return true;
        }
    }
    
    // If I didn't patch BeforeAttack, the VigorPower would be be consumed here.
    [HarmonyPatch(typeof(VigorPower), nameof(DoubleDamagePower.ModifyDamageAdditive), new Type[] { typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay) })]
    public class VigorPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CardModel? cardSource, Creature? dealer)
        {
            if (cardSource?.Keywords.Contains(SoraModKeywords.Link) == true && dealer == cardSource.Owner?.Creature)
            {
                return false;
            }
            return true;
        }
    }
    
    
    [HarmonyPatch(typeof(GigantificationPower), nameof(GigantificationPower.BeforeAttack), new Type[] { typeof(AttackCommand)})]
    public class GigantificationBeforeAttackPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(AttackCommand command, ref Task __result)
        {
            if (command.CardPlay?.Card.Keywords.Contains(SoraModKeywords.Link) == true)
            {
                // Skip the original method
                __result = Task.CompletedTask;
                return false;
            }
        
            // Run the original method normally for non-Link cards
            return true;
        }
    }
    
    
    [HarmonyPatch(typeof(GigantificationPower), nameof(GigantificationPower.ModifyDamageMultiplicative), new Type[] { typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay) })]
    public class GigantificationPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CardModel? cardSource, Creature? dealer)
        {
            if (cardSource?.Keywords.Contains(SoraModKeywords.Link) == true && dealer == cardSource.Owner?.Creature)
            {
                return false;
            }
            return true;
        }
    }
    
    [HarmonyPatch(typeof(DoubleDamagePower), nameof(DoubleDamagePower.ModifyDamageMultiplicative), new Type[] { typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay) })]
    public class DoubleDamagePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CardModel? cardSource, Creature? dealer, ref decimal __result)
        {
            if (cardSource?.Keywords.Contains(SoraModKeywords.Link) == true && dealer == cardSource.Owner?.Creature)
            {
                __result = 1M;   
                return false;    // Skip the original method
            }
            return true; // Run the original method if conditions aren't met
        }
    }
    
    [HarmonyPatch(typeof(TrackingPower), nameof(TrackingPower.ModifyDamageMultiplicative), new Type[] { typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay) })]
    public class TrackingPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CardModel? cardSource, Creature? dealer, ref decimal __result)
        {
            if (cardSource?.Keywords.Contains(SoraModKeywords.Link) == true && dealer == cardSource.Owner?.Creature)
            {
                __result = 1M;   
                return false;    // Skip the original method
            }
            return true; // Run the original method if conditions aren't met
        }
    }

    [HarmonyPatch(typeof(LethalityPower), nameof(LethalityPower.ModifyDamageMultiplicative), new Type[] { typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay) })]
    public class LethalityPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CardModel? cardSource, Creature? dealer, ref decimal __result)
        {
            if (cardSource?.Keywords.Contains(SoraModKeywords.Link) == true && dealer == cardSource.Owner?.Creature)
            {
                __result = 1M;   
                return false;    // Skip the original method
            }
            return true; // Run the original method if conditions aren't met
        }
    }
    
    [HarmonyPatch(typeof(OneForAllPower), nameof(OneForAllPower.ModifyDamageAdditive), new Type[] { typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay) })]
    public class OneForAllPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CardModel? cardSource, Creature? dealer, ref decimal __result)
        {
            if (cardSource?.Keywords.Contains(SoraModKeywords.Link) == true && dealer == cardSource.Owner?.Creature)
            {
                __result = 0M;   
                return false;    // Skip the original method
            }
            return true; // Run the original method if conditions aren't met
        }
    }
    
    [HarmonyPatch(typeof(ShrinkPower), nameof(ShrinkPower.ModifyDamageMultiplicative), new Type[] { typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay) })]
    public class ShrinkPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CardModel? cardSource, Creature? dealer, ref decimal __result)
        {
            if (cardSource?.Keywords.Contains(SoraModKeywords.Link) == true && dealer == cardSource.Owner?.Creature)
            {
                __result = 1M;   
                return false;
            }
            return true;
        }
    }
}