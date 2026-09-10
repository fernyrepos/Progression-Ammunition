using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ProgressionAmmunition
{
    [HotSwappable]
    [StaticConstructorOnStartup]
    public class Gizmo_Ammo : Gizmo
    {
        private readonly CompAmmo compAmmo;
        private static bool draggingBar;

        private const float GizmoWidth = 212f;
        private static readonly Texture2D BarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.34f, 0.42f, 0.43f));
        private static readonly Texture2D BarHighlightTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.43f, 0.54f, 0.55f));
        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.03f, 0.035f, 0.05f));
        private static readonly Texture2D TargetTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.74f, 0.97f, 0.8f));
        private static readonly Texture2D ReloadIcon = ContentFinder<Texture2D>.Get("UI/ReloadButton");

        public Gizmo_Ammo(CompAmmo compAmmo)
        {
            this.compAmmo = compAmmo;
            Order = -95f;
        }

        public override float GetWidth(float maxWidth)
        {
            return GizmoWidth;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            var rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            var innerRect = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);

            var iconRect = new Rect(innerRect.x, innerRect.y, innerRect.height, innerRect.height);
            Widgets.DrawMenuSection(iconRect);
            Widgets.DrawTextureFitted(iconRect, compAmmo.parent.def.uiIcon, 0.85f);
            TooltipHandler.TipRegion(iconRect, compAmmo.parent.LabelCap);

            var contentRect = new Rect(iconRect.xMax + 4f, innerRect.y, innerRect.xMax - iconRect.xMax - 4f, innerRect.height);

            Text.Font = GameFont.Small;
            var labelRect = new Rect(contentRect.x, contentRect.y + 4f, contentRect.width - 28f, Text.LineHeight);
            Widgets.Label(labelRect, compAmmo.parent.LabelNoParenthesisCap.Truncate(labelRect.width));

            var buttonRect = new Rect(contentRect.xMax - 24f, contentRect.y + 4f, 24f, 24f);
            if (Widgets.ButtonImage(buttonRect, ReloadIcon))
            {
                TryOrderReloadJob();
            }
            TooltipHandler.TipRegion(buttonRect, "PA_ForceReloadDesc".Translate());

            var barRect = new Rect(contentRect.x, contentRect.y + 36f, contentRect.width, 24f);
            var curAmmoPct = (float)compAmmo.CurAmmo / compAmmo.MaxAmmo;
            var threshold = compAmmo.autoReloadThreshold;

            Widgets.DraggableBar(barRect, BarTex, BarHighlightTex, EmptyBarTex, TargetTex, ref draggingBar, curAmmoPct, ref threshold);
            compAmmo.autoReloadThreshold = threshold;

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(barRect, "PA_AmmoFraction".Translate(compAmmo.CurAmmo, compAmmo.MaxAmmo));
            Text.Anchor = TextAnchor.UpperLeft;

            TooltipHandler.TipRegion(barRect, "PA_AmmoGizmoTooltip".Translate(compAmmo.CurAmmo, compAmmo.MaxAmmo, (compAmmo.autoReloadThreshold * 100f).ToString("F0")));

            return new GizmoResult(GizmoState.Clear);
        }

        private void TryOrderReloadJob()
        {
            var pawn = compAmmo.Holder;

            if (compAmmo.CurAmmo >= compAmmo.MaxAmmo)
            {
                Messages.Message("PA_AmmoAlreadyFull".Translate(), pawn, MessageTypeDefOf.RejectInput, false);
            }
            else
            {
                var consumable = compAmmo.ConsumableDef;
                if (consumable != null)
                {
                    var item = pawn.inventory.innerContainer.FirstOrFallback(t => t.def == consumable);
                    if (item != null)
                    {
                        pawn.inventory.innerContainer.Take(item, 1).Destroy();
                        compAmmo.RefillAmmo();
                        compAmmo.PlayReloadSound(pawn);
                        return;
                    }
                }

                var targetBuilding = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.Touch, TraverseParms.For(pawn), 9999f, t => t is Building_AmmoRecharger recharger && recharger.IsForbidden(pawn) is false && recharger.CanRecharge(compAmmo) && pawn.CanReserve(recharger));

                if (targetBuilding != null)
                {
                    var job = JobMaker.MakeJob(DefsOf.PA_ReloadAtBuilding, targetBuilding);
                    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                }
                else
                {
                    Messages.Message("PA_NoRechargerAvailable".Translate(compAmmo.WeaponAmmoType.ToStringHuman()), pawn, MessageTypeDefOf.RejectInput, false);
                }
            }
        }
    }

}
