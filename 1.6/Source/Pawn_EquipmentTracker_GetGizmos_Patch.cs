using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.GetGizmos))]
    public static class Pawn_EquipmentTracker_GetGizmos_Patch
    {
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn_EquipmentTracker __instance)
        {
            foreach (var g in __result)
            {
                yield return g;
            }
            var ammoComp = __instance.Primary?.TryGetComp<CompAmmo>();
            if (ammoComp != null)
            {
                foreach (var g in ammoComp.GetAmmoGizmos())
                {
                    yield return g;
                }
            }
        }
    }
}
