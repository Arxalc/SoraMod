using System.Reflection;
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Enchantments;

namespace BaseLib.Patches.Features;

[HarmonyPatch]
public class MeldPatch
{
    static MethodBase TargetMethod()
    {
        var method = AccessTools.Method(typeof(CardModel), "OnPlayWrapper",
            new Type[]
            {
                typeof(PlayerChoiceContext),
                typeof(Creature),
                typeof(bool),
                typeof(ResourceInfo),
                typeof(bool)
            });
        
        return method;
    }

    // Reassign __result so the game awaits our wrapper instead of the original Task
    [HarmonyPostfix]
    public static void Postfix(
        ref Task __result,
        CardModel __instance,
        PlayerChoiceContext choiceContext)
    {
        var original = __result;
        __result = PostfixAsync(original, __instance, choiceContext);
    }

    // Gets around the stack problem of the second melded card playing.
    private static async Task PostfixAsync(
        Task originalTask,
        CardModel __instance,
        PlayerChoiceContext choiceContext)
    {
        await originalTask;

        if (__instance.Enchantment is not Meld meld) return;
        if (meld.MeldTargetId == null) return;
        if (meld.Status == EnchantmentStatus.Disabled) return;
            
        Log.Error($"### MELD WRAPPER ### targetId={meld.MeldTargetId} upgraded={meld.MeldTargetWasUpgraded}");

        // ✅ Create a fresh combat instance from the ModelId
        
        var canonical = ModelDb.GetByIdOrNull<CardModel>(meld.MeldTargetId);
        if (canonical == null)
        {
            Log.Error($"### MELD: no card found for {meld.MeldTargetId} ###");
            return;
        }

        var combatCard = __instance.CombatState.CreateCard(canonical, __instance.Owner);
        if (combatCard == null)
        {
            Log.Error($"### MELD: CreateCard returned null ###");
            return;
        }

        if (meld.MeldTargetWasUpgraded && combatCard.IsUpgradable && !combatCard.IsUpgraded)
        {
            combatCard.UpgradeInternal();
        }
        
        // To prevent Exhaust-ible and Power cards from playing over and over
        bool shouldExhaust = combatCard.Keywords.Contains(CardKeyword.Exhaust) || combatCard.Type == CardType.Power;

        Log.Error($"### MELD PLAYING ### card={combatCard.Id} pile={combatCard.Pile?.Type.ToString() ?? "null"}");

        await CardCmd.AutoPlay(new ThrowingPlayerChoiceContext(), combatCard, null);
        
        if (shouldExhaust)
        {
            await CardCmd.Exhaust(new ThrowingPlayerChoiceContext(), combatCard);
            meld.Status = EnchantmentStatus.Disabled;
        }
        else
        {
            await CardPileCmd.RemoveFromCombat(combatCard);
        }

        Log.Error($"### MELD COMPLETE ###");
    }
}
