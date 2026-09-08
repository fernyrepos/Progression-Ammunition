using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ProgressionAmmunition
{
    public class Building_AmmoRecharger : Building
    {
        public AmmoType RechargerAmmoType => def.GetModExtension<AmmoExtension>().ammoType.Value;

        public CompPowerTrader PowerTrader => GetComp<CompPowerTrader>();

        public bool IsPowered => PowerTrader == null || PowerTrader.PowerOn;

        public bool CanRecharge(CompAmmo ammo)
        {
            if (ammo.WeaponAmmoType != RechargerAmmoType || IsPowered is false || ammo.CurAmmo >= ammo.MaxAmmo) return false;
            return true;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (var opt in base.GetFloatMenuOptions(selPawn))
            {
                yield return opt;
            }

            if (selPawn.IsColonistPlayerControlled is false) yield break;

            var weapon = selPawn.equipment?.Primary;
            var ammoComp = weapon?.TryGetComp<CompAmmo>();
            if (ammoComp == null || ammoComp.WeaponAmmoType != RechargerAmmoType)
            {
                yield return new FloatMenuOption("PA_IncompatibleWeapon".Translate(), null);
                yield break;
            }

            if (IsPowered is false)
            {
                yield return new FloatMenuOption("PA_RechargerUnpowered".Translate(), null);
                yield break;
            }

            if (ammoComp.CurAmmo >= ammoComp.MaxAmmo)
            {
                yield return new FloatMenuOption("PA_AmmoAlreadyFull".Translate(), null);
                yield break;
            }

            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("PA_ReloadWeaponAtBuilding".Translate(weapon.Label), () => selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(DefsOf.PA_ReloadAtBuilding, this), JobTag.Misc)), selPawn, this);
        }
    }
}
