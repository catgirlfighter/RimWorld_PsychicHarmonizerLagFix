using RimWorld;
using UnityEngine;
using Verse;

namespace PsychicHarmonizerLagFix
{
    public class Thought_PsychicHarmonizerCached : Thought_PsychicHarmonizer
    {
        public override float MoodOffset()
        {
            float? num = cachedMoodOffset;
            if (num != null && num.Value != float.PositiveInfinity)
            {
                return num.Value;
            }
            num = cachedMoodOffset = new float?(getMoodOffset());
            return num.Value;
        }

        public float getMoodOffset()
        {
            float num;
            if ((num = Thought_MemoryMoodOffset()) == 0f)
            {
                return 0f;
            }
            float num2 = Mathf.Lerp(-1f, 1f, harmonizer.pawn.needs.mood.CurLevel);
            float statValue = StatExtension.GetStatValue(harmonizer.pawn, StatDefOf.PsychicSensitivity, true);
            return num * num2 * statValue;
        }

        public override bool ShouldDiscard
        {
            get
            {
                bool shouldDiscard = base.ShouldDiscard;
                if (shouldDiscard)
                {
                    cachedMoodOffset = null;
                    return shouldDiscard;
                }
                cachedMoodOffset = new float?(getMoodOffset());
                return shouldDiscard;
            }
        }

        public override bool VisibleInNeedsTab
        {
            get
            {
                return CurStage.visible && cachedMoodOffset != null;
            }
        }

        private float Thought_MemoryMoodOffset()
        {
            float num = ThoughtMoodOffset();
            num *= moodPowerFactor;
            num += moodOffset;
            if (def.lerpMoodToZero)
            {
                num *= 1f - age / DurationTicks;
            }
            return num;
        }

        private float ThoughtMoodOffset()
        {
            //base.base.MoodOffset or Thought_Memory.MoodOffset
            //what can you do if you want to skip base.MoodOffset, but keep, base.base and base.base.base?
            if (CurStage == null)
            {
                Log.Error($"CurStage is null while ShouldDiscard is false on {def.defName} for {pawn}");
                return 0f;
            }
            if (ThoughtUtility.ThoughtNullified(pawn, def))
            {
                return 0f;
            }
            float num = BaseMoodOffset;
            if (def.effectMultiplyingStat != null)
            {
                num *= StatExtension.GetStatValue(pawn, def.effectMultiplyingStat);
            }
            if (def.Worker != null)
            {
                num *=  def.Worker.MoodMultiplier(pawn);
            }
            return num;
        }

        private float? cachedMoodOffset = new float?(float.PositiveInfinity);
    }
}
