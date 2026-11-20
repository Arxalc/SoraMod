using BaseLib.Cards.Variables;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode;
using SoraMod.SoraModCode.Keywords;

#nullable enable
namespace BaseLib.Patches.Features;

[HarmonyPatch(typeof(AfflictionModel), nameof(AfflictionModel.OverlayPath), MethodType.Getter)]
public class PatchOverlayPath
{
    public static void Postfix(AfflictionModel __instance, ref string __result)
    {
        string entryId = __instance.Id.Entry.ToLowerInvariant();
        __result = MainFile.ResPath + "/scenes/cards/overlays/afflictions/" + entryId + ".tscn";
    } 
}