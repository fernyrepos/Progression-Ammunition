using HarmonyLib;
using RimWorld;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(Bill_Production), nameof(Bill_Production.ShouldDoNow))]
    public static class Bill_Production_ShouldDoNow_Patch
    {
        public static void Postfix(Bill_Production __instance, ref bool __result)
        {
            if (__result && ProgressionAmmunitionMod.Enabled is false && RefillUtility.IsRefill(__instance.recipe?.ProducedThingDef))
            {
                __result = false;
            }
        }
    }
}
