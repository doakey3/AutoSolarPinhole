using UnityEngine;
using Verse;

namespace AutoSolarPinhole
{
    public class Building_SolarPinholeSpot : Building
    {
        public bool NeedsPinholeRefreshed()
        {
            if (Map != null)
            {
                foreach (Thing thing in GenRadial.RadialDistinctThingsAround(Position, Map, 1f, useCenter: true))
                {
                    if (thing.def.defName == "SolarPinhole")
                    {
                        var comp = GetComp<CompSolarPinholeSettings>();
                        double threshold = (comp?.Threshold ?? 4.5f) * 60000;

                        var ageComp = thing.TryGetComp<CompSolarPinholeAgeTracker>();
                        if (ageComp != null)
                        {
                            return ageComp.Age >= threshold;
                        }
                    }
                }
            }

            // No pinhole found — needs one
            return true;
        }

        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            GenDraw.DrawRadiusRing(Position, 9f);
        }
    }

    public class PlaceWorker_SolarPinhole : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            GenDraw.DrawRadiusRing(center, 9f);
        }
    }
}
