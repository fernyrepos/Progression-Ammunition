using System.Collections.Generic;
using Verse;

namespace ProgressionAmmunition
{
    [StaticConstructorOnStartup]
    public static class AmmoCompInjector
    {
        static AmmoCompInjector()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.IsRangedWeapon is false)
                {
                    continue;
                }
                if (AmmoExtension.IsAmmoDisabledFor(def))
                {
                    def.comps?.RemoveAll(c => c is CompProperties_Ammo);
                    continue;
                }
                def.comps ??= new List<CompProperties>();
                if (def.comps.Any(c => c is CompProperties_Ammo))
                {
                    continue;
                }
                def.comps.Add(new CompProperties_Ammo());
            }
        }
    }
}
