using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    public static class AmmoTypeUtility
    {
        public static AmmoType WeaponAmmoType(this ThingDef weaponDef)
        {
            var ext = weaponDef.GetModExtension<AmmoExtension>();
            if (ext?.ammoType != null)
            {
                return ext.ammoType.Value;
            }
            if (weaponDef.techLevel <= TechLevel.Medieval)
            {
                return AmmoType.Arrow;
            }
            if (weaponDef.techLevel <= TechLevel.Industrial)
            {
                return AmmoType.Ammo;
            }
            return AmmoType.Charge;
        }

        public static string ToStringHuman(this AmmoType ammoType)
        {
            return ammoType switch
            {
                AmmoType.Arrow => "PA_AmmoType_Arrow".Translate(),
                AmmoType.Ammo => "PA_AmmoType_Ammo".Translate(),
                AmmoType.Charge => "PA_AmmoType_Charge".Translate(),
                _ => ammoType.ToString()
            };
        }
    }
}
