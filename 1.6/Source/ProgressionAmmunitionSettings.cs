using Verse;

namespace ProgressionAmmunition
{
    public class ProgressionAmmunitionSettings : ModSettings
    {
        public bool showOnlyDrafted = true;
        public int baselineMaxAmmo = 30;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref showOnlyDrafted, "showOnlyDrafted", true);
            Scribe_Values.Look(ref baselineMaxAmmo, "baselineMaxAmmo", 30);
        }
    }
}
