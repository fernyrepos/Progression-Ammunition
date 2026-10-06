using HarmonyLib;
using Verse;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(PlayDataLoader), nameof(PlayDataLoader.HotReloadDefs))]
    public static class PlayDataLoader_HotReloadDefs_Patch
    {
        public static void Postfix()
        {
            OutOfAmmoUtility.Notify_DefsHotReloaded();
        }
    }
}
