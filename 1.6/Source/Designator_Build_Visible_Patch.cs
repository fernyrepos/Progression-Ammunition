using HarmonyLib;
using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(Designator_Build), nameof(Designator_Build.Visible), MethodType.Getter)]
    public static class Designator_Build_Visible_Patch
    {
        public static void Postfix(Designator_Build __instance, ref bool __result)
        {
            if (__result && ProgressionAmmunitionMod.Enabled is false && __instance.PlacingDef is ThingDef def && typeof(Building_AmmoRecharger).IsAssignableFrom(def.thingClass))
            {
                __result = false;
            }
        }
    }
}
