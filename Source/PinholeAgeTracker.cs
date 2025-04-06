using Verse;

namespace AutoSolarPinhole
{
    public class CompProperties_SolarPinholeAgeTracker : CompProperties
    {
        public CompProperties_SolarPinholeAgeTracker()
        {
            this.compClass = typeof(CompSolarPinholeAgeTracker);
        }
    }

    public class CompSolarPinholeAgeTracker : ThingComp
    {
        public int spawnTick = -1;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                spawnTick = Find.TickManager.TicksGame;
            }
        }

        public int Age => spawnTick >= 0 ? Find.TickManager.TicksGame - spawnTick : int.MaxValue;
    }
}
