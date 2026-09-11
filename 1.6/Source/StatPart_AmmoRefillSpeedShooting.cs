using RimWorld;
using Verse;

namespace ProgressionAmmunition
{
    public class StatPart_AmmoRefillSpeedShooting : StatPart
    {
        private const float AmmoRefillSpeedBonusPerShootingSkillLevel = 0.025f;

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!req.HasThing)
                return;

            Pawn pawn = req.Thing as Pawn;
            if (pawn?.skills?.GetSkill(SkillDefOf.Shooting) == null)
                return;

            float bonus = pawn.skills.GetSkill(SkillDefOf.Shooting).Level * AmmoRefillSpeedBonusPerShootingSkillLevel;
            val += bonus;
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (!req.HasThing)
                return null;

            Pawn pawn = req.Thing as Pawn;
            if (pawn?.skills?.GetSkill(SkillDefOf.Shooting) == null)
                return null;

            float bonus = pawn.skills.GetSkill(SkillDefOf.Shooting).Level * AmmoRefillSpeedBonusPerShootingSkillLevel;
            return $"Shooting skill: +{bonus * 100f:0.#}%";
        }
    }
}