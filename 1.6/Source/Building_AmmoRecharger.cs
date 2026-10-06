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
            if (ProgressionAmmunitionMod.Enabled is false || ammo.WeaponAmmoType != RechargerAmmoType || IsPowered is false || ammo.CurAmmo >= ammo.MaxAmmo)
                return false;

            if (!HasInfiniteRefills() && this.TryGetComp<CompRefuelable>(out var compRefuelable))
            {
                return compRefuelable.Fuel >= 1f;
            }

            return true;
        }

        public bool HasInfiniteRefills()
        {
            if (ProgressionAmmunitionMod.settings.refillBuildingsAreInfinite)
                return true;

            if (ProgressionAmmunitionMod.settings.weaponChargersAreInfinite && RechargerAmmoType == AmmoType.Charge)
                return true;

            return false;
        }

        public void ConsumeAmmoRefill()
        {
            if (HasInfiniteRefills())
                return;

            if (this.TryGetComp<CompRefuelable>(out var compRefuelable))
            {
                compRefuelable.ConsumeFuel(1f);
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (var opt in base.GetFloatMenuOptions(selPawn))
            {
                yield return opt;
            }

            if (!RefillUtility.DoesPawnUseAmmo(selPawn) || !selPawn.IsColonistPlayerControlled)
                yield break;

            var weapon = selPawn.equipment?.Primary;
            var ammoComp = weapon?.TryGetComp<CompAmmo>();
            if (ammoComp == null)
            {
                yield break;
            }
            else if (ammoComp.WeaponAmmoType != RechargerAmmoType)
            {
                yield return new FloatMenuOption("PA_IncompatibleWeapon".Translate(), null);
            }
            else if (IsPowered is false)
            {
                yield return new FloatMenuOption("PA_RechargerUnpowered".Translate(), null);
            }
            else if (ammoComp.CurAmmo >= ammoComp.MaxAmmo)
            {
                yield return new FloatMenuOption("PA_AmmoAlreadyFull".Translate(), null);
            }
            else if (!HasInfiniteRefills() && this.TryGetComp<CompRefuelable>(out var compRefuelable) && compRefuelable.Fuel < 1f)
            {
                yield return new FloatMenuOption("PA_AmmoBoxOutOfAmmo".Translate(), null);
            }
            else if (selPawn.CanReserve(this) is false)
            {
                yield return new FloatMenuOption("PA_RechargerInUse".Translate(), null);
            }
            else
            {
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("PA_ReloadWeaponAtBuilding".Translate(weapon.Label), () => selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(DefsOf.PA_ReloadAtBuilding, this), JobTag.Misc)), selPawn, this);
            }
        }
    }
}
