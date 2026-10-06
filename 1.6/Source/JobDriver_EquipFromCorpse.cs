using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ProgressionAmmunition
{
    public class JobDriver_EquipFromCorpse : JobDriver
    {
        private const TargetIndex CorpseInd = TargetIndex.A;

        private Corpse TargetCorpse => (Corpse)job.GetTarget(CorpseInd).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetCorpse, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(CorpseInd);
            yield return Toils_Goto.GotoThing(CorpseInd, PathEndMode.Touch);

            Toil wait = Toils_General.Wait(60);
            wait.WithProgressBarToilDelay(CorpseInd);
            yield return wait;

            yield return Toils_General.Do(() =>
            {
                if (TargetCorpse?.InnerPawn == null)
                    return;

                Pawn innerPawn = TargetCorpse.InnerPawn;

                // equipped weapon
                if (innerPawn.equipment != null)
                {
                    foreach (ThingWithComps eq in innerPawn.equipment.AllEquipmentListForReading)
                    {
                        if (eq == null || !OutOfAmmoUtility.IsUsableWeaponForPawn(eq, pawn))
                            continue;

                        innerPawn.equipment.Remove(eq);
                        if (pawn.equipment?.Primary != null)
                            OutOfAmmoUtility.TryStowOrDropWeapon(pawn);

                        if (pawn.equipment != null && pawn.equipment.Primary == null)
                            pawn.equipment.AddEquipment(eq);
                        else if (pawn.inventory?.innerContainer != null)
                            pawn.inventory.innerContainer.TryAdd(eq);
                        break;
                    }
                }

                // inventory
                if (innerPawn.inventory?.innerContainer != null)
                {
                    foreach (Thing invItem in innerPawn.inventory.innerContainer)
                    {
                        if (invItem == null || !OutOfAmmoUtility.IsUsableWeaponForPawn(invItem, pawn))
                            continue;

                        if (invItem is ThingWithComps invTwp)
                        {
                            innerPawn.inventory.innerContainer.Remove(invTwp);
                            if (pawn.equipment?.Primary != null)
                                OutOfAmmoUtility.TryStowOrDropWeapon(pawn);

                            if (pawn.equipment != null && pawn.equipment.Primary == null)
                                pawn.equipment.AddEquipment(invTwp);
                            else if (pawn.inventory?.innerContainer != null)
                                pawn.inventory.innerContainer.TryAdd(invTwp);
                            break;
                        }
                    }
                }
            });
        }

        public override string GetReport()
        {
            return "PA_ReportEquipFromCorpse".Translate(TargetCorpse.LabelCap);
        }
    }
}