using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    public class ScenPart_StartingAmmoStation : ScenPart_StartingThing_Defined
    {
        public void Configure(ThingDef building, ThingDef buildingStuff)
        {
            def = DefsOf.StartingThing_Defined;
            thingDef = building;
            stuff = buildingStuff;
            count = 1;
        }

        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            if (ProgressionAmmunitionMod.AddRefillBuildingsToScenarios is false)
            {
                return Enumerable.Empty<string>();
            }
            return base.GetSummaryListEntries(tag);
        }

        public override IEnumerable<Thing> PlayerStartingThings()
        {
            if (ProgressionAmmunitionMod.AddRefillBuildingsToScenarios is false)
            {
                return Enumerable.Empty<Thing>();
            }
            return base.PlayerStartingThings();
        }
    }
}
