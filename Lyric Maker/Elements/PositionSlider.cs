using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Lyric_Maker.Elements
{
    /// <summary>
    /// Represents a Position Slider, it has three events : Started, Delta and Completed.
    /// </summary>
    public class PositionSlider : Slider
    {
        //@Delegate
        /// <summary> Occurs when the value changed starts. </summary>
        public event EventHandler ValueChangedStarted;
        /// <summary> Occurs when value changed. </summary>
        public event RangeBaseValueChangedEventHandler ValueChangedDelta;
        /// <summary> Occurs when the value changed is complete. </summary>
        public event EventHandler ValueChangedCompleted;

        VisualStateGroup CommonStates;
        VisualState Normal;
        VisualState PointerOver;
        VisualState Pressed;
        VisualState Disabled;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //VisualStateGroup
            if (!(this.CommonStates is null)) this.CommonStates.CurrentStateChanged -= this.CommonStates_CurrentStateChanged;
            this.CommonStates = base.GetTemplateChild(nameof(CommonStates)) as VisualStateGroup;
            if (!(this.CommonStates is null)) this.CommonStates.CurrentStateChanged += this.CommonStates_CurrentStateChanged;
            //VisualState
            this.Normal = base.GetTemplateChild(nameof(Normal)) as VisualState;
            this.PointerOver = base.GetTemplateChild(nameof(PointerOver)) as VisualState;
            this.Pressed = base.GetTemplateChild(nameof(Pressed)) as VisualState;
            this.Disabled = base.GetTemplateChild(nameof(Disabled)) as VisualState;
        }

        private void CommonStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.OldState != this.Pressed && e.NewState == this.Pressed)
            {
                this.ValueChangedStarted?.Invoke(sender, new EventArgs());//Delegate
                base.ValueChanged += this.ValueChangedDelta;//Add Delegate
            }
            if (e.OldState == this.Pressed && e.NewState != this.Pressed)
            {
                this.ValueChangedCompleted?.Invoke(sender, new EventArgs());//Delegate
                base.ValueChanged -= this.ValueChangedDelta;//Remove Delegate
            }
        }
    }
}
