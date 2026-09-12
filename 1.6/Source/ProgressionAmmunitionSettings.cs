using Verse;

namespace ProgressionAmmunition
{
    public class ProgressionAmmunitionSettings : ModSettings
    {
        public bool enableMod = true;
        public bool showOnlyDrafted = true;
        public bool autoRefillWithConsumable = false;
        public bool addRefillBuildingsToScenarios = true;
        public int baselineMaxAmmo = 30;
        public bool scaleMaxAmmoByBurstShotCount = true;
        public float animalRefillChance = 0f;
        public float neolithicRefillChance = 5f;
        public float medievalRefillChance = 15f;
        public float industrialRefillChance = 15f;
        public float spacerRefillChance = 15f;
        public float ultraRefillChance = 15f;
        public float archotechRefillChance = 15f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref enableMod, "enableMod", true);
            Scribe_Values.Look(ref showOnlyDrafted, "showOnlyDrafted", true);
            Scribe_Values.Look(ref autoRefillWithConsumable, "autoRefillWithConsumable", false);
            Scribe_Values.Look(ref addRefillBuildingsToScenarios, "addRefillBuildingsToScenarios", true);
            Scribe_Values.Look(ref baselineMaxAmmo, "baselineMaxAmmo", 30);
            Scribe_Values.Look(ref scaleMaxAmmoByBurstShotCount, "scaleMaxAmmoByBurstShotCount", true);
            Scribe_Values.Look(ref animalRefillChance, "animalRefillChance", 0f);
            Scribe_Values.Look(ref neolithicRefillChance, "neolithicRefillChance", 5f);
            Scribe_Values.Look(ref medievalRefillChance, "medievalRefillChance", 15f);
            Scribe_Values.Look(ref industrialRefillChance, "industrialRefillChance", 15f);
            Scribe_Values.Look(ref spacerRefillChance, "spacerRefillChance", 15f);
            Scribe_Values.Look(ref ultraRefillChance, "ultraRefillChance", 15f);
            Scribe_Values.Look(ref archotechRefillChance, "archotechRefillChance", 15f);
        }
    }
}
