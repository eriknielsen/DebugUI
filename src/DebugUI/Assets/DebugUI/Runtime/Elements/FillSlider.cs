using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace DebugUI.UIElements
{
    public class FillSlider : Slider
    {
        public sealed new class UxmlFactory : UxmlFactory<FillSlider, UxmlTraits> { }

        readonly VisualElement filler;
        readonly Label valueLabel;

        public string Format { get; set; } = "{0:F2}";
        public float stepSize = 0.1f;

        float previousValue;

        public FillSlider() : base()
        {
            var dragger = this.Q("unity-dragger");
            var tracker = this.Q("unity-tracker");
            var dragContainer = this.Q("unity-drag-container");

            valueLabel = new Label();
            valueLabel.AddToClassList(UssClasses.debug_ui_slider__label);
            Add(valueLabel);

            filler = new VisualElement();
            filler.AddToClassList(UssClasses.debug_ui_slider__filler);
            tracker.Add(filler);

            dragContainer.AddToClassList("debug-ui-slider__drag-container");

            OnValueChanged(value);
            this.RegisterValueChangedCallback(x => OnValueChanged(x.newValue));
        }

        public void ForceUpdateValue(float x)
        {
            this.value = x;
            OnValueChanged(x);
        }

        void OnValueChanged(float x)
        {
            float steppedValue = (x / stepSize);

            if (previousValue > x)
            {
                // If user is lowering the value, round down
                steppedValue = Mathf.Floor(steppedValue);
            }
            else
            {
                // If user is increasing the value, round up
                steppedValue = Mathf.Ceil(steppedValue);
            }

            value = steppedValue * stepSize;
            filler.style.width = Length.Percent(Mathf.InverseLerp(lowValue, highValue, value) * 100f);
            valueLabel.text = string.Format(Format, value);

            previousValue = value;
        }
    }

    public class FillSliderInt : SliderInt
    {
        public sealed new class UxmlFactory : UxmlFactory<FillSliderInt, UxmlTraits> { }

        readonly VisualElement filler;
        readonly Label valueLabel;

        public string Format { get; set; } = "{0}";

        public int stepSize = 1;

        int previousValue;

        public FillSliderInt() : base()
        {
            var dragger = this.Q("unity-dragger");
            var tracker = this.Q("unity-tracker");
            var dragContainer = this.Q("unity-drag-container");

            valueLabel = new Label();
            valueLabel.AddToClassList(UssClasses.debug_ui_slider__label);
            Add(valueLabel);

            filler = new VisualElement();
            filler.AddToClassList(UssClasses.debug_ui_slider__filler);
            tracker.Add(filler);

            dragContainer.AddToClassList("debug-ui-slider__drag-container");

            OnValueChanged(value);
            this.RegisterValueChangedCallback(x => OnValueChanged(x.newValue));
        }

        public void ForceUpdateValue(int x)
        {
            this.value = x;
            OnValueChanged(x);
        }

        void OnValueChanged(int x)
        {
            float steppedValue = ((float)x / (float)stepSize);

            if (previousValue > x)
            {
                // If user is lowering the value, round down
                steppedValue = (Mathf.Floor(steppedValue));
            }
            else
            {
                // If user is increasing the value, round up
                steppedValue = (Mathf.Ceil(steppedValue));
            }

            value = (int)(steppedValue * stepSize);

            filler.style.width = Length.Percent(Mathf.InverseLerp(lowValue, highValue, x) * 100f);
            valueLabel.text = string.Format(Format, x);

            previousValue = value;
        }
    }
}