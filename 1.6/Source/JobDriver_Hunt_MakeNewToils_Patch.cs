using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(JobDriver_Hunt), "MakeNewToils")]
    public static class JobDriver_Hunt_MakeNewToils_Patch
    {
        public static void Postfix(JobDriver_Hunt __instance)
        {
            __instance.FailOn(() =>
            {
                var pawn = __instance.pawn;
                if (pawn.IsColonist is false || pawn.Faction != Faction.OfPlayer) return false;
                var comp = pawn.equipment?.Primary?.TryGetComp<CompAmmo>();
                return comp != null && comp.CurAmmo < 1;
            });
        }
    }
}
