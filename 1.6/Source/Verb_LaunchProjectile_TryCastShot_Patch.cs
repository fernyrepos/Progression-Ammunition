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
            if (__instance.CasterPawn is Pawn pawn && RefillUtility.DoesPawnUseAmmo(pawn))
            {
                CompAmmo comp = __instance.EquipmentSource?.TryGetComp<CompAmmo>();
                if (comp != null && comp.IsOutOfAmmo)
                {
                    return false;
                }
            }
            return true;
        }

        public static void Postfix(Verb_LaunchProjectile __instance, bool __result)
        {
            if (__result && __instance.CasterPawn is Pawn pawn && RefillUtility.DoesPawnUseAmmo(pawn))
            {
                CompAmmo comp = __instance.EquipmentSource?.TryGetComp<CompAmmo>();
                if (comp != null)
                {
                    comp.ConsumeAmmo();
                    if (comp.IsOutOfAmmo)
                    {
                        MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "PA_MoteOutOfAmmo".Translate());

                        if (pawn.IsColonist && pawn.Faction == Faction.OfPlayer)
                        {
                            if (ProgressionAmmunitionMod.settings.autoRefillWithConsumable)
                            {
                                comp.TryRefillAmmoFromConsumable();
                            }
                        }
                        else
                        {
                            if (comp.TryRefillAmmoFromConsumable())
                                return;

                            OutOfAmmoUtility.TryStowOrDropWeapon(pawn);
                            if (!OutOfAmmoUtility.TryEquipOtherWeapon(pawn))
                            {
                                if (ProgressionAmmunitionMod.settings.canAIPawnsScavengeForWeapons)
                                    OutOfAmmoUtility.TryScavengeWeapon(pawn);
                            }
                        }
                    }
                }
            }
        }
    }
}
