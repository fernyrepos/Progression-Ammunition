using HarmonyLib;
using RimWorld;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.CompInspectStringExtra))]
    public static class CompRefuelable_CompInspectStringExtra_Patch
    {
        public static bool Prefix(CompRefuelable __instance, ref string __result)
        {
            if (__instance.parent is Building_AmmoRecharger ammoCharger && ammoCharger.HasInfiniteRefills())
            {
                __result = null;
                return false;
            }

            return true;
        }
    }
}
