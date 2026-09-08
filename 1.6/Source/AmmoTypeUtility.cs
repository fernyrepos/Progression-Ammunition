using Verse;

namespace ProgressionAmmunition
{
    public static class AmmoTypeUtility
    {
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
