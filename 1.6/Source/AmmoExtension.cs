using Verse;

namespace ProgressionAmmunition
{
    public class AmmoExtension : DefModExtension
    {
        public AmmoType? ammoType;
        public int maxAmmo = -1;
        public bool disableAmmo;

        public static bool IsAmmoDisabledFor(ThingDef def)
        {
            return def?.GetModExtension<AmmoExtension>()?.disableAmmo ?? false;
        }
    }
}
