using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ProgressionAmmunition
{
    public class JobDriver_ReloadAtBuilding : JobDriver
    {
        private const TargetIndex BuildingInd = TargetIndex.A;
        private const int ReloadTicks = 600;

        private Building_AmmoRecharger Recharger => (Building_AmmoRecharger)job.GetTarget(BuildingInd).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(BuildingInd), job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(BuildingInd);
            this.FailOn(() =>
            {
                var comp = pawn.equipment?.Primary?.TryGetComp<CompAmmo>();
                return comp == null || !Recharger.CanRecharge(comp);
            });

            yield return Toils_Goto.GotoThing(BuildingInd, PathEndMode.Touch);

            var waitToil = Toils_General.Wait(ReloadTicks, BuildingInd);
            waitToil.WithProgressBarToilDelay(BuildingInd);
            yield return waitToil;

            yield return Toils_General.Do(() =>
            {
                var comp = pawn.equipment.Primary.TryGetComp<CompAmmo>();
                comp.RefillAmmo();
                comp.PlayReloadSound(pawn);
            });
        }

        public override string GetReport()
        {
            return "PA_ReportReloadingAtBuilding".Translate(Recharger.LabelCap);
        }
    }
}
