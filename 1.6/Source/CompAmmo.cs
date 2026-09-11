using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ProgressionAmmunition
{
    public class CompAmmo : ThingComp
    {
        private int curAmmo = -1;
        public float autoReloadThreshold = 0.5f;

        public Pawn Holder => (parent.ParentHolder as Pawn_EquipmentTracker)?.pawn;

        public int MaxAmmo
        {
            get
            {
                var ext = parent.def.GetModExtension<AmmoExtension>();
                float ammo = (ext != null && ext.maxAmmo > 0) ? ext.maxAmmo : ProgressionAmmunitionMod.settings.baselineMaxAmmo;

                var pawn = Holder;
                if (pawn != null)
                {
                    StatDef maxAmmoFactorStat = DefsOf.PA_MaxAmmoFactor;
                    float factor = (maxAmmoFactorStat != null) ? pawn.GetStatValue(maxAmmoFactorStat) : 1f;
                    ammo *= factor;
                }

                return Mathf.Max(1, Mathf.RoundToInt(ammo));
            }
        }

        public AmmoType WeaponAmmoType
        {
            get
            {
                var ext = parent.def.GetModExtension<AmmoExtension>();
                if (ext?.ammoType != null)
                {
                    return ext.ammoType.Value;
                }
                if (parent.def.techLevel <= TechLevel.Medieval)
                {
                    return AmmoType.Arrow;
                }
                if (parent.def.techLevel <= TechLevel.Industrial)
                {
                    return AmmoType.Ammo;
                }
                return AmmoType.Charge;
            }
        }

        public int CurAmmo
        {
            get
            {
                if (curAmmo < 0)
                {
                    curAmmo = MaxAmmo;
                }
                return curAmmo;
            }
            set => curAmmo = Mathf.Clamp(value, 0, MaxAmmo);
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
            curAmmo = MaxAmmo;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref curAmmo, "curAmmo", -1);
            Scribe_Values.Look(ref autoReloadThreshold, "autoReloadThreshold", 0.5f);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && curAmmo < 0)
            {
                curAmmo = MaxAmmo;
            }
        }

        public void ConsumeAmmo(int amount = 1)
        {
            CurAmmo -= amount;
        }

        public void RefillAmmo()
        {
            CurAmmo = MaxAmmo;
        }

        public ThingDef ConsumableDef
        {
            get
            {
                switch (WeaponAmmoType)
                {
                    case AmmoType.Arrow:
                        return DefsOf.PA_ArrowRefill;
                    case AmmoType.Ammo:
                        return DefsOf.PA_AmmoRefill;
                    case AmmoType.Charge:
                        return DefsOf.PA_ChargeRefill;
                    default:
                        return null;
                }
            }
        }

        public SoundDef ReloadSound
        {
            get
            {
                switch (WeaponAmmoType)
                {
                    case AmmoType.Arrow:
                        return DefsOf.PA_ReloadArrows;
                    case AmmoType.Charge:
                        return DefsOf.PA_ChargeWeapon;
                    default:
                        return DefsOf.Standard_Reload;
                }
            }
        }

        public void PlayReloadSound(Pawn pawn)
        {
            if (pawn?.Map == null)
            {
                return;
            }
            ReloadSound?.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
        }

        public IEnumerable<Gizmo> GetAmmoGizmos()
        {
            if (ProgressionAmmunitionMod.Enabled is false || parent.def.IsRangedWeapon is false)
            {
                yield break;
            }

            var pawn = Holder;
            if (pawn == null || pawn.IsColonist is false || pawn.Faction != Faction.OfPlayer)
            {
                yield break;
            }

            if (ProgressionAmmunitionMod.settings.showOnlyDrafted is false || pawn.Drafted)
            {
                yield return new Gizmo_Ammo(this);
            }

            var consumable = ConsumableDef;
            if (consumable != null)
            {
                var item = pawn.inventory.innerContainer.FirstOrFallback(t => t.def == consumable);
                if (item != null)
                {
                    var cmd = new Command_Action
                    {
                        defaultLabel = "PA_RefillWithConsumable".Translate(consumable.label),
                        defaultDesc = "PA_RefillWithConsumableDesc".Translate(consumable.label),
                        icon = consumable.uiIcon,
                        action = () =>
                        {
                            TryRefillAmmoFromConsumable();
                        }
                    };
                    if (CurAmmo >= MaxAmmo)
                    {
                        cmd.Disable("PA_AmmoAlreadyFull".Translate());
                    }
                    yield return cmd;
                }
            }
        }

        public void TryRefillAmmoFromConsumable()
        {
            if (Holder is Pawn pawn)
            {
                var consumable = ConsumableDef;
                if (consumable == null)
                    return;

                var item = pawn.inventory.innerContainer.FirstOrFallback(t => t.def == consumable);
                if (item == null)
                    return;

                pawn.inventory.innerContainer.Take(item, 1).Destroy();
                RefillAmmo();
                PlayReloadSound(pawn);
            }
        }

        public override string CompInspectStringExtra()
        {
            if (ProgressionAmmunitionMod.Enabled is false || parent.def.IsRangedWeapon is false) return null;
            var pawn = Holder;
            if (pawn == null || pawn.IsColonist is false || pawn.Faction != Faction.OfPlayer)
            {
                return null;
            }
            return "PA_AmmoRemaining".Translate(CurAmmo, MaxAmmo);
        }
    }
}
