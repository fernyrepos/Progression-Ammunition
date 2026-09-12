using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    [StaticConstructorOnStartup]
    public static class ScenarioAmmoStationAutoPatcher
    {
        static ScenarioAmmoStationAutoPatcher()
        {
            foreach (var scenarioDef in DefDatabase<ScenarioDef>.AllDefs)
            {
                TryAddAmmoStations(scenarioDef.scenario);
            }
        }

        public static void TryAddAmmoStations(Scenario scenario)
        {
            if (scenario?.parts == null || scenario.parts.Any(p => p is ScenPart_StartingAmmoStation))
            {
                return;
            }
            foreach (var ammoType in StartingAmmoTypes(scenario).ToList())
            {
                var building = StationFor(ammoType);
                if (building == null)
                {
                    continue;
                }
                var part = new ScenPart_StartingAmmoStation();
                part.Configure(building, StuffFor(building));
                scenario.parts.Add(part);
            }
        }

        private static IEnumerable<AmmoType> StartingAmmoTypes(Scenario scenario)
        {
            return scenario.parts
                .Where(p => p is not ScenPart_StartingAmmoStation && (p is ScenPart_StartingThing_Defined || p is ScenPart_ScatterThingsNearPlayerStart))
                .Select(p => ((ScenPart_ThingCount)p).thingDef)
                .Where(def => def != null && def.IsRangedWeapon && AmmoExtension.IsAmmoDisabledFor(def) is false)
                .Select(def => def.WeaponAmmoType())
                .Distinct();
        }

        private static ThingDef StationFor(AmmoType ammoType)
        {
            return DefDatabase<ThingDef>.AllDefs.FirstOrDefault(def =>
                typeof(Building_AmmoRecharger).IsAssignableFrom(def.thingClass)
                && def.BuildableByPlayer
                && def.GetModExtension<AmmoExtension>()?.ammoType == ammoType);
        }

        private static ThingDef StuffFor(ThingDef building)
        {
            if (building.MadeFromStuff is false)
            {
                return null;
            }
            if (GenStuff.AllowedStuffsFor(building).Contains(ThingDefOf.WoodLog))
            {
                return ThingDefOf.WoodLog;
            }
            return GenStuff.DefaultStuffFor(building);
        }
    }
}
