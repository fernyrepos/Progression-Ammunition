using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ProgressionAmmunition
{
    public class OutOfAmmoUtility
    {
        public const float BackupWeaponChance = 0.33f;
        public const float BackupWeaponRangedChance = 0.1f; // otherwise melee
        public const float BackupWeaponMaxMarketValueFactor = 0.75f;
        public static List<ThingDef> cacheBackupWeapons;

        public static ThingDef GetBackupWeaponDefForPawn(Pawn pawn)
        {
            if (pawn?.equipment?.Primary?.def?.IsRangedWeapon != true || AmmoExtension.IsAmmoDisabledFor(pawn.equipment.Primary.def) || !RefillUtility.DoesPawnUseAmmo(pawn))
                return null;

            float maxMarketValue = pawn?.equipment?.Primary?.def?.BaseMarketValue * BackupWeaponMaxMarketValueFactor ?? 0f;

            if (Rand.Chance(BackupWeaponChance))
            {
                TechLevel techLevel = pawn?.Faction?.def?.techLevel ?? pawn.equipment?.Primary?.def?.techLevel ?? TechLevel.Neolithic;
                return GetBackupWeaponForTechLevel(techLevel, maxMarketValue, Rand.Chance(BackupWeaponRangedChance));
            }
            return null;
        }

        public static ThingDef GetBackupWeaponForTechLevel(TechLevel techLevel, float maxMarketValue, bool isRanged = false)
        {
            if (cacheBackupWeapons == null)
                BuildBackupWeaponCache();

            List<ThingDef> filteredWeapons = [.. cacheBackupWeapons.Where(def =>
               (isRanged ? def.IsRangedWeapon : def.IsMeleeWeapon) &&
                def.techLevel <= techLevel &&
                def.BaseMarketValue <= maxMarketValue
            )];

            return filteredWeapons.NullOrEmpty() ? null : filteredWeapons.RandomElement();
        }

        private static void BuildBackupWeaponCache()
        {
            if (cacheBackupWeapons != null)
                return;

            cacheBackupWeapons = [];
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
            {
                AmmoExtension ext = def.GetModExtension<AmmoExtension>();
                if ((def.IsMeleeWeapon || def.IsRangedWeapon) && ext != null && ext.canBeBackupWeapon)
                    cacheBackupWeapons.Add(def);
            }
        }

        public static void Notify_DefsHotReloaded() => cacheBackupWeapons = null;

        public static void TryStowOrDropWeapon(Pawn pawn)
        {
            if (pawn?.inventory?.innerContainer == null || pawn?.equipment?.Primary == null)
                return;

            ThingWithComps currentPrimary = pawn.equipment.Primary;
            pawn.equipment.Remove(currentPrimary);
            if (pawn.inventory.innerContainer.TryAdd(currentPrimary))
                return;

            if (pawn.Position.InBounds(pawn.Map))
            {
                if (!pawn.equipment.TryDropEquipment(currentPrimary, out _, pawn.Position))
                {
                    if (!currentPrimary.Destroyed)
                        currentPrimary.Destroy();
                }
            }
        }

        public static bool TryEquipOtherWeapon(Pawn pawn, bool stowOrDropPrimaryWeapon = true)
        {
            if (pawn?.inventory?.innerContainer == null)
                return false;

            foreach (Thing item in pawn.inventory.innerContainer)
            {
                if (item.def.IsWeapon && item is ThingWithComps weaponToEquip)
                {
                    if (weaponToEquip.TryGetComp<CompAmmo>(out var comp) && comp.IsOutOfAmmo)
                        continue;

                    ThingWithComps currentPrimary = pawn.equipment.Primary;
                    if (currentPrimary != null && stowOrDropPrimaryWeapon)
                    {
                        TryStowOrDropWeapon(pawn);
                    }
                    currentPrimary = pawn.equipment.Primary;
                    if (currentPrimary == null)
                    {
                        pawn.inventory.innerContainer.Remove(weaponToEquip);
                        pawn.equipment.AddEquipment(weaponToEquip);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
