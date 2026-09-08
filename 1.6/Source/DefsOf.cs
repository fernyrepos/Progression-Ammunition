using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    [DefOf]
    public static class DefsOf
    {
        public static JobDef PA_ReloadAtBuilding;
        public static ThingDef PA_ArrowRefill;
        public static ThingDef PA_AmmoRefill;
        public static ThingDef PA_ChargeRefill;
        public static SoundDef Standard_Reload;
        static DefsOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
        }
    }
}
