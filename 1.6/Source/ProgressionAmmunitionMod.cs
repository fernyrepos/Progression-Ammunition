using HarmonyLib;
using UnityEngine;
using Verse;

namespace ProgressionAmmunition
{
    public class ProgressionAmmunitionMod : Mod
    {
        public static ProgressionAmmunitionSettings settings;

        public static bool Enabled => settings == null || settings.enableMod;

        public static bool AddRefillBuildingsToScenarios => Enabled && (settings == null || settings.addRefillBuildingsToScenarios);

        public ProgressionAmmunitionMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<ProgressionAmmunitionSettings>();
            new Harmony("ProgressionAmmunitionMod").PatchAll();
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }

        private static Vector2 scrollPosition;
        private static float scrollViewHeight;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var viewRect = new Rect(0f, 0f, inRect.width - 16f, Mathf.Max(scrollViewHeight, inRect.height));
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);
            var listing = new Listing_Standard { maxOneColumn = true };
            listing.Begin(new Rect(0f, 0f, viewRect.width, 99999f));
            listing.CheckboxLabeled("PA_EnableMod".Translate(), ref settings.enableMod, "PA_EnableModDesc".Translate());
            listing.CheckboxLabeled("PA_OnlyColonistsUseAmmo".Translate(), ref settings.onlyColonistsUseAmmo, "PA_OnlyColonistsUseAmmoDesc".Translate());
            if (!settings.onlyColonistsUseAmmo)
            {
                listing.CheckboxLabeled("PA_CanAIPawnsBringBackupWeapons".Translate(), ref settings.canAIPawnsBringBackupWeapons);
            }
            listing.GapLine();
            listing.CheckboxLabeled("PA_ShowOnlyDrafted".Translate(), ref settings.showOnlyDrafted);
            listing.Gap();
            listing.CheckboxLabeled("PA_RefillBuildingsAreInfinite".Translate(), ref settings.refillBuildingsAreInfinite, "PA_RefillBuildingsAreInfiniteDesc".Translate());
            if (!settings.refillBuildingsAreInfinite)
            {
                listing.CheckboxLabeled("PA_WeaponChargersAreInfinite".Translate(), ref settings.weaponChargersAreInfinite);
            }
            listing.CheckboxLabeled("PA_AutoRefillWithConsumable".Translate(), ref settings.autoRefillWithConsumable);
            listing.Gap();
            listing.CheckboxLabeled("PA_AddRefillBuildingsToScenarios".Translate(), ref settings.addRefillBuildingsToScenarios, "PA_AddRefillBuildingsToScenariosDesc".Translate());
            listing.GapLine();
            listing.Label("PA_BaselineMaxAmmo".Translate(settings.baselineMaxAmmo));
            settings.baselineMaxAmmo = (int)listing.Slider(settings.baselineMaxAmmo, 1, 300);
            listing.CheckboxLabeled("PA_ScaleMaxAmmoByBurstShotCount".Translate(), ref settings.scaleMaxAmmoByBurstShotCount, "PA_ScaleMaxAmmoByBurstShotCountDesc".Translate());
            listing.GapLine();
            listing.Label("PA_SpawnRefillHeader".Translate());
            ChanceSlider(listing, "PA_AnimalRefillChance", ref settings.animalRefillChance);
            ChanceSlider(listing, "PA_NeolithicRefillChance", ref settings.neolithicRefillChance);
            ChanceSlider(listing, "PA_MedievalRefillChance", ref settings.medievalRefillChance);
            ChanceSlider(listing, "PA_IndustrialRefillChance", ref settings.industrialRefillChance);
            ChanceSlider(listing, "PA_SpacerRefillChance", ref settings.spacerRefillChance);
            ChanceSlider(listing, "PA_UltraRefillChance", ref settings.ultraRefillChance);
            ChanceSlider(listing, "PA_ArchotechRefillChance", ref settings.archotechRefillChance);
            scrollViewHeight = listing.CurHeight + 12f;
            listing.End();
            Widgets.EndScrollView();
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
