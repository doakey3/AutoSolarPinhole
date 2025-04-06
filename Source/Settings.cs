using UnityEngine;
using Verse;
using System.Collections.Generic;

namespace AutoSolarPinhole
{
    public class CompProperties_SolarPinholeSettings : CompProperties
    {
        public CompProperties_SolarPinholeSettings()
        {
            compClass = typeof(CompSolarPinholeSettings);
        }
    }

    public class CompSolarPinholeSettings : ThingComp
    {
        private float threshold = 3.5f;

        public float Threshold => threshold;

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref threshold, "solarPinholeThreshold", 3.5f);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                defaultLabel = "Set Refresh Threshold",
                defaultDesc = "Adjust the wait time before the Solar Pinhole Spot should request a refreshed pinhole psycast. Note: Solar Pinholes last 5 days.",
                icon = ContentFinder<Texture2D>.Get("UI/Icons/RefreshThreshold"),
                action = () =>
                {
                    Find.WindowStack.Add(new Dialog_FloatSlider(
                        value: threshold,
                        min: 1f,
                        max: 5f,
                        onConfirm: val => threshold = val,
                        labelFor: val => $"Refresh threshold: {val:F1} days"
                    ));
                }
            };
        }
    }
}
