using HarmonyLib;
using UnityEngine;
using Verse;

namespace ProgressionAmmunition
{
    public class ProgressionAmmunitionMod : Mod
    {
        public static ProgressionAmmunitionSettings settings;

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
            listing.CheckboxLabeled("PA_ShowOnlyDrafted".Translate(), ref settings.showOnlyDrafted);
            listing.Gap();
            listing.Label("PA_BaselineMaxAmmo".Translate(settings.baselineMaxAmmo));
            settings.baselineMaxAmmo = (int)listing.Slider(settings.baselineMaxAmmo, 1, 300);
            listing.End();
        }
    }
}
