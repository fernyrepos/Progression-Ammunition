using RimWorld;
using Verse;
using Verse.AI;

namespace ProgressionAmmunition
{
    public class JobGiver_AutoReloadAmmo : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.IsColonist is false || pawn.Drafted || pawn.Faction != Faction.OfPlayer || pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) is false)
            {
                return null;
            }

            var weapon = pawn.equipment?.Primary;
            var comp = weapon?.TryGetComp<CompAmmo>();
            if (comp == null || comp.CurAmmo >= comp.MaxAmmo)
            {
                return null;
            }

            var ratio = (float)comp.CurAmmo / comp.MaxAmmo;
            if (ratio > comp.autoReloadThreshold)
            {
                return null;
            }

            var recharger = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.Touch, TraverseParms.For(pawn), 9999f, t => t is Building_AmmoRecharger b && b.IsForbidden(pawn) is false && b.CanRecharge(comp) && pawn.CanReserve(b));
            if (recharger == null) return null;

            return JobMaker.MakeJob(DefsOf.PA_ReloadAtBuilding, recharger);
        }
    }
}
