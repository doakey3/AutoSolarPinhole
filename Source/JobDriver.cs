using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;

namespace AutoSolarPinhole
{
    public class JobDriver_CastSolarPinhole : JobDriver
    {
        // We use A for the target building, B for the position to stand and cast
        private const TargetIndex TargetInd = TargetIndex.A;
        private const float Range = 25f;

        // Reserve the casting target (SolarPinholeSpot)
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // Fail early if the building target is gone or inaccessible
            this.FailOnDespawnedNullOrForbidden(TargetInd);

            // Step 1: Move to a valid position from which psycast can be cast
            Toil gotoCastPos = ToilMaker.MakeToil("GotoCastPosition");
            gotoCastPos.initAction = () =>
            {
                LocalTargetInfo target = job.targetA;
                IntVec3 castPos;

                // Find a position from which the pawn can cast the psycast on the target
                bool found = CastPositionFinder.TryFindCastPosition(new CastPositionRequest
                {
                    caster = pawn,
                    target = target.Thing,
                    verb = GetSolarPinholeVerb(),
                    maxRangeFromTarget = Range,
                    wantCoverFromTarget = false
                }, out castPos);

                if (!found)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                // Save the cast position in the job (not a Thing, just a cell)
                job.SetTarget(TargetIndex.B, castPos);
                pawn.pather.StartPath(castPos, PathEndMode.OnCell);
            };
            gotoCastPos.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            yield return gotoCastPos;

            // Step 2: Optional warmup time with progress bar
            yield return Toils_General.Wait(30).WithProgressBarToilDelay(TargetInd);

            // Step 3: Perform the psycast (if still valid)
            Toil castToil = ToilMaker.MakeToil("CastPsycast");
            castToil.initAction = () =>
            {
                // Get the ability def
                AbilityDef solarPinholeDef = DefDatabase<AbilityDef>.GetNamed("SolarPinhole", false);
                if (solarPinholeDef == null)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                // Get the actual ability object from the pawn
                Ability ability = pawn.abilities?.GetAbility(solarPinholeDef);
                if (ability == null || !ability.CanCast)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                // Make sure the target is still valid and in range
                LocalTargetInfo target = job.targetA;
                if (!ability.verb.CanHitTargetFrom(pawn.Position, target))
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                // Cast the psycast!
                ability.verb.TryStartCastOn(target);
            };
            castToil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return castToil;
        }

        // Helper method to fetch the correct verb used by Solar Pinhole ability
        private Verb GetSolarPinholeVerb()
        {
            AbilityDef def = DefDatabase<AbilityDef>.GetNamed("SolarPinhole", false);
            Ability ab = pawn.abilities?.GetAbility(def);
            return ab?.verb;
        }
    }
}
