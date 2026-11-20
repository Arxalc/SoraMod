using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Encounters;

namespace BaseLib.Patches.Features;

// Really gotta double check timing on this.
[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
public class HovertipsPatch
{
    public static void Postfix(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance.Affliction != null && __instance.Affliction is Enshroud)
            __result = HoverTipFactory.FromAffliction<Enshroud>();
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.Title), MethodType.Getter)]
public class TitlePatch
{
    public static void Postfix(CardModel __instance, ref string __result)
    {
        if (__instance.Affliction != null && __instance.Affliction is Enshroud)
            __result = "? ? ?";
    }
}