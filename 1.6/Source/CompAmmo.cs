using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ProgressionAmmunition
{
    public class CompAmmo : ThingComp
    {
        private const float BurstShotMaxAmmoMultiplier = 1f; // only used for weapons without AmmoExtension.maxAmmo
        private const int MaxBurstShotCountForAmmoScaling = 10;
        private int curAmmo = -1;
        public float autoReloadThreshold = 1f;

        public Pawn Holder => (parent.ParentHolder as Pawn_EquipmentTracker)?.pawn;

        public int MaxAmmo
        {
            get
            {
                var ext = parent.def.GetModExtension<AmmoExtension>();
                float burstShotBonusMaxAmmo = ProgressionAmmunitionMod.settings.scaleMaxAmmoByBurstShotCount is false ? 0f : ProgressionAmmunitionMod.settings.baselineMaxAmmo * Mathf.Max((Math.Clamp(GetBurstShotCount(), 1, MaxBurstShotCountForAmmoScaling) - 1) * BurstShotMaxAmmoMultiplier, 0);
                float ammo = (ext != null && ext.maxAmmo > 0) ? ext.maxAmmo : ProgressionAmmunitionMod.settings.baselineMaxAmmo + burstShotBonusMaxAmmo;

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

        private int GetBurstShotCount()
        {
            // TODO VWE has unique traits that can override Verb_Shoot and its burstShotCount. Those overrides are not handled right now

            VerbProperties verbProps = parent.def.verbs.FirstOrDefault(v => v.verbClass != null && typeof(Verb_Shoot).IsAssignableFrom(v.verbClass));
            int burstShotCount = (verbProps != null) ? verbProps.burstShotCount : 1;
            return burstShotCount;
        }

        public AmmoType WeaponAmmoType => parent.def.WeaponAmmoType();

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

        public bool IsOutOfAmmo => CurAmmo <= 0;

        public override void PostPostMake()
        {
            base.PostPostMake();
            curAmmo = MaxAmmo;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref curAmmo, "curAmmo", -1);
            Scribe_Values.Look(ref autoReloadThreshold, "autoReloadThreshold", 1f);
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
            var pawn = Holder;
            if (pawn == null || !parent.def.IsRangedWeapon || !RefillUtility.DoesPawnUseAmmo(pawn))
            {
                yield break;
            }

            if (!ProgressionAmmunitionMod.settings.showOnlyDrafted || pawn.Drafted || (DebugSettings.ShowDevGizmos && !pawn.IsPlayerControlled))
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

            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: Set ammo to 0",
                    action = () =>
                    {
                        CurAmmo = 0;
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "DEV: Ammo +1",
                    action = () =>
                    {
                        CurAmmo += 1;
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "DEV: Set ammo to max",
                    action = () =>
                    {
                        CurAmmo = MaxAmmo;
                    }
                };
            }
        }

        public bool TryRefillAmmoFromConsumable()
        {
            if (Holder is Pawn pawn)
            {
                var consumable = ConsumableDef;
                if (consumable == null)
                    return false;

                var item = pawn.inventory.innerContainer.FirstOrFallback(t => t.def == consumable);
                if (item == null)
                    return false;

                pawn.inventory.innerContainer.Take(item, 1).Destroy();
                RefillAmmo();
                PlayReloadSound(pawn);
                return true;
            }
            return false;
        }

        public override string CompInspectStringExtra()
        {
            var pawn = Holder;
            if (pawn != null || !parent.def.IsRangedWeapon || AmmoExtension.IsAmmoDisabledFor(parent.def))
            {
                return null;
            }

            return "PA_AmmoRemaining".Translate(CurAmmo, MaxAmmo);
        }
    }
}
