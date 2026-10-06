using HarmonyLib;
using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GenerateGearFor))]
    public static class PawnGenerator_GenerateGearFor_Patch
    {
        public static void Postfix(Pawn pawn)
        {
            if (pawn?.inventory == null || pawn?.Faction?.IsPlayer == true || !RefillUtility.DoesPawnUseAmmo(pawn))
            {
                return;
            }

            // Refills
            var refill = GetRefillItemDefForPawn(pawn);
            if (refill != null)
            {
                if (Rand.Chance(ChanceForRefill(pawn) / 100f))
                {
                    Thing thing = ThingMaker.MakeThing(refill);
                    thing.stackCount = 1;
                    if (!pawn.inventory.innerContainer.TryAdd(thing))
                        thing.Destroy();
                }
            }

            // Backup Weapons
            if (ProgressionAmmunitionMod.settings.canAIPawnsBringBackupWeapons)
            {
                ThingDef backupWeaponDef = OutOfAmmoUtility.GetBackupWeaponDefForPawn(pawn);
                if (backupWeaponDef != null)
                {
                    Thing backupWeapon = ThingMaker.MakeThing(backupWeaponDef, backupWeaponDef.MadeFromStuff ? GenStuff.DefaultStuffFor(backupWeaponDef) : null);
                    if (!pawn.inventory.innerContainer.TryAdd(backupWeapon))
                        backupWeapon.Destroy();
                }
            }
        }

        private static float ChanceForRefill(Pawn pawn)
        {
            var settings = ProgressionAmmunitionMod.settings;
            if (pawn.RaceProps.Animal)
            {
                return settings.animalRefillChance;
            }
            switch (GetTechLevelForPawn(pawn))
            {
                case TechLevel.Neolithic:
                    return settings.neolithicRefillChance;
                case TechLevel.Medieval:
                    return settings.medievalRefillChance;
                case TechLevel.Industrial:
                    return settings.industrialRefillChance;
                case TechLevel.Spacer:
                    return settings.spacerRefillChance;
                case TechLevel.Ultra:
                    return settings.ultraRefillChance;
                case TechLevel.Archotech:
                    return settings.archotechRefillChance;
                default:
                    return 0f;
            }
        }

        private static TechLevel GetTechLevelForPawn(Pawn pawn)
        {
            var factionLevel = pawn.Faction?.def?.techLevel ?? TechLevel.Undefined;
            if (factionLevel > TechLevel.Animal)
            {
                return factionLevel;
            }
            return pawn.equipment?.Primary?.def?.techLevel ?? TechLevel.Undefined;
        }

        private static ThingDef GetRefillItemDefForPawn(Pawn pawn)
        {
            if (pawn.RaceProps.Animal)
            {
                return DefsOf.PA_ArrowRefill;
            }
            var weapon = pawn.equipment?.Primary;
            if (weapon == null || !weapon.def.IsRangedWeapon || AmmoExtension.IsAmmoDisabledFor(weapon.def))
            {
                return null;
            }
            var comp = weapon.TryGetComp<CompAmmo>();
            if (comp != null)
            {
                return comp.ConsumableDef;
            }
            if (weapon.def.techLevel <= TechLevel.Medieval)
            {
                return DefsOf.PA_ArrowRefill;
            }
            if (weapon.def.techLevel <= TechLevel.Industrial)
            {
                return DefsOf.PA_AmmoRefill;
            }
            return DefsOf.PA_ChargeRefill;
        }
    }
}
