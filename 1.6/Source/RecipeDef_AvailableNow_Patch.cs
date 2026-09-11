using HarmonyLib;
using Verse;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(RecipeDef), nameof(RecipeDef.AvailableNow), MethodType.Getter)]
    public static class RecipeDef_AvailableNow_Patch
    {
        public static void Postfix(RecipeDef __instance, ref bool __result)
        {
            if (__result && ProgressionAmmunitionMod.Enabled is false && RefillUtility.IsRefill(__instance.ProducedThingDef))
            {
                __result = false;
            }
        }
    }
}
