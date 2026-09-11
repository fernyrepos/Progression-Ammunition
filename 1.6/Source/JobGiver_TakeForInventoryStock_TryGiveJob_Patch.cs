using HarmonyLib;
using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(JobGiver_TakeForInventoryStock), "TryGiveJob")]
    public static class JobGiver_TakeForInventoryStock_TryGiveJob_Patch
    {
        public static void Prefix(Pawn pawn, out InventoryStockEntry __state)
        {
            __state = null;
            if (ProgressionAmmunitionMod.Enabled)
            {
                return;
            }
            var entries = pawn?.inventoryStock?.stockEntries;
            if (entries != null && entries.TryGetValue(DefsOf.PA_AmmoStock, out var entry))
            {
                __state = entry;
                entries.Remove(DefsOf.PA_AmmoStock);
            }
        }

        public static void Finalizer(Pawn pawn, InventoryStockEntry __state)
        {
            if (__state != null && pawn?.inventoryStock?.stockEntries != null)
            {
                pawn.inventoryStock.stockEntries[DefsOf.PA_AmmoStock] = __state;
            }
        }
    }
}
