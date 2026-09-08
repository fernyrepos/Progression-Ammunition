using HarmonyLib;
using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(Verb), nameof(Verb.Available))]
    public static class Verb_Available_Patch
    {
        public static void Postfix(Verb __instance, ref bool __result)
        {
            if (__result && __instance.IsMeleeAttack is false && __instance.CasterPawn is Pawn pawn && pawn.IsColonist && pawn.Faction == Faction.OfPlayer)
            {
                var comp = __instance.EquipmentSource?.TryGetComp<CompAmmo>();
                if (comp != null && comp.CurAmmo <= 0)
                {
                    __result = false;
                }
            }
        }
    }
}
