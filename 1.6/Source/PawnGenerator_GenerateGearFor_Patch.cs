using HarmonyLib;
using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    [HarmonyPatch(typeof(PawnGenerator), "GenerateGearFor")]
    public static class PawnGenerator_GenerateGearFor_Patch
    {
        public static void Postfix(Pawn pawn)
        {
            if (pawn?.inventory == null || (pawn.Faction != null && pawn.Faction.IsPlayer))
            {
                return;
            }

            var refill = RefillFor(pawn);
            if (refill == null)
            {
                return;
            }

            if (Rand.Chance(ChanceFor(pawn) / 100f) is false)
            {
                return;
            }

            var thing = ThingMaker.MakeThing(refill);
            thing.stackCount = 1;
            if (pawn.inventory.innerContainer.TryAdd(thing) is false)
            {
                thing.Destroy();
            }
        }

        private static float ChanceFor(Pawn pawn)
        {
            var settings = ProgressionAmmunitionMod.settings;
            if (pawn.RaceProps.Animal)
            {
                return settings.animalRefillChance;
            }
            switch (TechLevelFor(pawn))
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

        private static TechLevel TechLevelFor(Pawn pawn)
        {
            var factionLevel = pawn.Faction?.def?.techLevel ?? TechLevel.Undefined;
            if (factionLevel > TechLevel.Animal)
            {
                return factionLevel;
            }
            return pawn.equipment?.Primary?.def?.techLevel ?? TechLevel.Undefined;
        }

        private static ThingDef RefillFor(Pawn pawn)
        {
            if (pawn.RaceProps.Animal)
            {
                return DefsOf.PA_ArrowRefill;
            }
            var weapon = pawn.equipment?.Primary;
            if (weapon == null || weapon.def.IsRangedWeapon is false || AmmoExtension.IsAmmoDisabledFor(weapon.def))
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
