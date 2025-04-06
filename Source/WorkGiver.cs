using RimWorld;
using Verse;
using Verse.AI;

namespace AutoSolarPinhole
{
    public class WorkGiver_SolarPinhole : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override ThingRequest PotentialWorkThingRequest =>
            ThingRequest.ForDef(DefDatabase<ThingDef>.GetNamed("SolarPinholeSpot"));

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            AbilityDef solarPinholeDef = DefDatabase<AbilityDef>.GetNamedSilentFail("SolarPinhole");
            if (solarPinholeDef == null || pawn.abilities == null)
                return true;

            Ability ability = pawn.abilities.GetAbility(solarPinholeDef);
            if (ability == null || !ability.CanCast)
                return true;

            if (pawn.psychicEntropy?.EntropyValue > pawn.psychicEntropy?.MaxEntropy - 5f)
                return true;

            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_SolarPinholeSpot spot = t as Building_SolarPinholeSpot;
            if (spot == null || !spot.NeedsPinholeRefreshed())
                return null;

            AbilityDef solarDef = DefDatabase<AbilityDef>.GetNamed("SolarPinhole", errorOnFail: false);
            if (solarDef == null)
                return null;

            Ability ability = pawn.abilities?.GetAbility(solarDef);
            if (ability == null || !ability.CanCast)
                return null;

            var entropy = pawn.psychicEntropy;
            if (entropy == null)
                return null;

            // ❗️Fix: Use EntropyValue instead of CurrentEntropy
            float projectedHeat = entropy.EntropyValue + solarDef.EntropyGain;
            if (projectedHeat > entropy.MaxEntropy)
                return null;

            // Psyfocus is a fraction from 0.0 to 1.0
            if (entropy.CurrentPsyfocus < solarDef.PsyfocusCostRange.max)
                return null;

            return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("CastSolarPinhole"), t);
        }
    }
}
