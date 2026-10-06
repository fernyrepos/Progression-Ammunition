using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.CompGetGizmosExtra))]
    public static class CompRefuelable_CompGetGizmosExtra_Patch
    {
        public static bool Prefix(CompRefuelable __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance.parent is Building_AmmoRecharger ammoCharger && ammoCharger.HasInfiniteRefills())
            {
                __result = Enumerable.Empty<Gizmo>();
                return false;
            }

            return true;
        }
    }
}
