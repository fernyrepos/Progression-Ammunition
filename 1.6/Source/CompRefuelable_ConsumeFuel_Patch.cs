using HarmonyLib;
using RimWorld;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.ConsumeFuel))]
    public static class CompRefuelable_ConsumeFuel_Patch
    {
        public static bool Prefix(CompRefuelable __instance)
        {
            if (__instance.parent is Building_AmmoRecharger ammoCharger && ammoCharger.HasInfiniteRefills())
            {
                return false;
            }

            return true;
        }
    }
}
