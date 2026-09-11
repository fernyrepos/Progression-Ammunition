using HarmonyLib;
using UnityEngine;
using Verse;

namespace ProgressionAmmunition
{
    public class ProgressionAmmunitionMod : Mod
    {
        public static ProgressionAmmunitionSettings settings;

        public static bool Enabled => settings == null || settings.enableMod;

        public ProgressionAmmunitionMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<ProgressionAmmunitionSettings>();
            new Harmony("ProgressionAmmunitionMod").PatchAll();
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("PA_EnableMod".Translate(), ref settings.enableMod, "PA_EnableModDesc".Translate());
            listing.GapLine();
            listing.CheckboxLabeled("PA_ShowOnlyDrafted".Translate(), ref settings.showOnlyDrafted);
            listing.Gap();
            listing.Label("PA_BaselineMaxAmmo".Translate(settings.baselineMaxAmmo));
            settings.baselineMaxAmmo = (int)listing.Slider(settings.baselineMaxAmmo, 1, 300);
            listing.GapLine();
            listing.Label("PA_SpawnRefillHeader".Translate());
            ChanceSlider(listing, "PA_AnimalRefillChance", ref settings.animalRefillChance);
            ChanceSlider(listing, "PA_NeolithicRefillChance", ref settings.neolithicRefillChance);
            ChanceSlider(listing, "PA_MedievalRefillChance", ref settings.medievalRefillChance);
            ChanceSlider(listing, "PA_IndustrialRefillChance", ref settings.industrialRefillChance);
            ChanceSlider(listing, "PA_SpacerRefillChance", ref settings.spacerRefillChance);
            ChanceSlider(listing, "PA_UltraRefillChance", ref settings.ultraRefillChance);
            ChanceSlider(listing, "PA_ArchotechRefillChance", ref settings.archotechRefillChance);
            listing.End();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            RefillUtility.ApplyEnabledState();
        }

        private static void ChanceSlider(Listing_Standard listing, string key, ref float value)
        {
            listing.Label(key.Translate(value.ToString("0")));
            value = Mathf.Round(listing.Slider(value, 0f, 100f));
        }
    }
}
