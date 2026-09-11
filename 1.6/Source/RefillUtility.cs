using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    [StaticConstructorOnStartup]
    public static class RefillUtility
    {
        private static readonly Dictionary<ThingDef, Tradeability> originalTradeability = new Dictionary<ThingDef, Tradeability>();

        static RefillUtility()
        {
            ApplyEnabledState();
        }

        public static IEnumerable<ThingDef> RefillDefs
        {
            get
            {
                yield return DefsOf.PA_ArrowRefill;
                yield return DefsOf.PA_AmmoRefill;
                yield return DefsOf.PA_ChargeRefill;
            }
        }

        public static bool IsRefill(ThingDef def)
        {
            return def != null && (def == DefsOf.PA_ArrowRefill || def == DefsOf.PA_AmmoRefill || def == DefsOf.PA_ChargeRefill);
        }

        public static void ApplyEnabledState()
        {
            foreach (var def in RefillDefs)
            {
                if (def == null)
                {
                    continue;
                }
                if (originalTradeability.TryGetValue(def, out var original) is false)
                {
                    original = def.tradeability;
                    originalTradeability[def] = original;
                }
                def.tradeability = ProgressionAmmunitionMod.Enabled ? original : WithoutTraderStock(original);
            }
        }

        private static Tradeability WithoutTraderStock(Tradeability tradeability)
        {
            switch (tradeability)
            {
                case Tradeability.All:
                    return Tradeability.Sellable;
                case Tradeability.Buyable:
                    return Tradeability.None;
                default:
                    return tradeability;
            }
        }
    }
}
