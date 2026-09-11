using RimWorld;
using Verse;
using Verse.AI;

namespace ProgressionAmmunition
{
    public class JobGiver_AutoReloadAmmo : ThinkNode_JobGiver
    {
        private const float ReloadPriority = 9.2f;

        public override float GetPriority(Pawn pawn)
        {
            return AmmoToReload(pawn) != null ? ReloadPriority : 0f;
        }

        public override Job TryGiveJob(Pawn pawn)
        {
            var comp = AmmoToReload(pawn);
            if (comp == null)
            {
                return null;
            }

            var recharger = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.Touch, TraverseParms.For(pawn), 9999f, t => t is Building_AmmoRecharger b && b.IsForbidden(pawn) is false && b.CanRecharge(comp) && pawn.CanReserve(b));
            if (recharger == null) return null;

            return JobMaker.MakeJob(DefsOf.PA_ReloadAtBuilding, recharger);
        }

        private static CompAmmo AmmoToReload(Pawn pawn)
        {
            if (ProgressionAmmunitionMod.Enabled is false || pawn.IsColonist is false || pawn.Drafted || pawn.Faction != Faction.OfPlayer || pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) is false)
            {
                return null;
            }

            var comp = pawn.equipment?.Primary?.TryGetComp<CompAmmo>();
            if (comp == null || comp.CurAmmo >= comp.MaxAmmo)
            {
                return null;
            }

            if ((float)comp.CurAmmo / comp.MaxAmmo > comp.autoReloadThreshold)
            {
                return null;
            }

            return comp;
        }
    }
}
