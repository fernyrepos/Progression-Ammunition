using HarmonyLib;
using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(Verb_LaunchProjectile), "TryCastShot")]
    public static class Verb_LaunchProjectile_TryCastShot_Patch
    {
        public static bool Prefix(Verb_LaunchProjectile __instance)
        {
            if (ProgressionAmmunitionMod.Enabled && __instance.CasterPawn is Pawn pawn && pawn.IsColonist && pawn.Faction == Faction.OfPlayer)
            {
                var comp = __instance.EquipmentSource?.TryGetComp<CompAmmo>();
                if (comp != null && comp.CurAmmo <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static void Postfix(Verb_LaunchProjectile __instance, bool __result)
        {
            if (__result && ProgressionAmmunitionMod.Enabled && __instance.CasterPawn is Pawn pawn && pawn.IsColonist && pawn.Faction == Faction.OfPlayer)
            {
                var comp = __instance.EquipmentSource?.TryGetComp<CompAmmo>();
                if (comp != null)
                {
                    comp.ConsumeAmmo();
                }
            }
        }
    }
}
