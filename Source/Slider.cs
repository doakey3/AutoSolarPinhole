using System;
using UnityEngine;
using Verse;

namespace AutoSolarPinhole
{
    public class Dialog_FloatSlider : Window
    {
        private float value;
        private readonly float min;
        private readonly float max;
        private readonly Action<float> onConfirm;
        private readonly Func<float, string> labelFor;

        public override Vector2 InitialSize => new Vector2(280f, 150f);

        public Dialog_FloatSlider(float value, float min, float max, Action<float> onConfirm, Func<float, string> labelFor)
        {
            this.value = value;
            this.min = min;
            this.max = max;
            this.onConfirm = onConfirm;
            this.labelFor = labelFor;

            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnAccept = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect sliderRect = new Rect(inRect.x, inRect.y + 20f, inRect.width, 40f);
            value = Widgets.HorizontalSlider(sliderRect, value, min, max, true, labelFor(value));

            if (Widgets.ButtonText(new Rect(inRect.x, inRect.y + 80f, inRect.width, 30f), "OK"))
            {
                onConfirm(value);
                Close();
            }
        }
    }
}
