using System;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Lyric_Maker.Times
{
    /// <summary>
    /// Represents a Button with a Minutes, Seconds, Milliseconds, 
    /// used to select the time you want.
    /// </summary>
    public sealed partial class TimeButton : Button
    {

        //@Converter
        public string TimeSpanToStringConverter(TimeSpan value) => value.ToString("mm':'ss'.'ff");


        #region DependencyProperty


        public TimeSpan Time
        {
            get => (TimeSpan)base.GetValue(TimeProperty);
            set => base.SetValue(TimeProperty, value);
        }
        /// <summary> Identifies the <see cref="TimeButton.Time"/> dependency property. </summary>
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(nameof(Time), typeof(TimeSpan), typeof(TimeButton), new PropertyMetadata(TimeSpan.Zero));


        public TimeSpan Maximum
        {
            get => (TimeSpan)base.GetValue(MaximumProperty);
            set => base.SetValue(MaximumProperty, value);
        }
        /// <summary> Identifies the <see cref="TimeButton.Maximum"/> dependency property. </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(TimeSpan), typeof(TimeButton), new PropertyMetadata(TimeSpan.MaxValue));


        public object MoveCommandParameter
        {
            get => (object)base.GetValue(MoveCommandParameterProperty);
            set => base.SetValue(MoveCommandParameterProperty, value);
        }
        /// <summary> Identifies the <see cref="TimeButton.MoveCommandParameter"/> dependency property. </summary>
        public static readonly DependencyProperty MoveCommandParameterProperty = DependencyProperty.Register(nameof(MoveCommandParameter), typeof(object), typeof(TimeButton), new PropertyMetadata(null));


        public ICommand MoveCommand
        {
            get => (ICommand)base.GetValue(MoveCommandProperty);
            set => base.SetValue(MoveCommandProperty, value);
        }
        /// <summary> Identifies the <see cref="TimeButton.MoveCommand"/> dependency property. </summary>
        public static readonly DependencyProperty MoveCommandProperty = DependencyProperty.Register(nameof(MoveCommand), typeof(ICommand), typeof(TimeButton), new PropertyMetadata(null));


        #endregion


        //@Construct
        /// <summary>
        /// Initializes a TimeButton. 
        /// </summary>
        public TimeButton()
        {
            this.InitializeComponent();
            this.ConstructFlowDirection();
            this.ConstructStrings();

            this.RootGrid.Loaded += (s, e) =>
            {
                if (this.RootGrid.Parent is FlyoutPresenter presenter)
                {
                    presenter.Padding = base.Padding;
                }
            };
            this.Flyout.Opened += (s, e) =>
            {
                this.MinuteNumberPicker.Index = this.Time.Minutes;
                this.SecondNumberPicker.Index = this.Time.Seconds;
                this.MillisecondNumberPicker.Index = this.Time.Milliseconds / 10;
            };

            this.CancelButton.Click += (s, e) => this.Flyout.Hide();
            this.OKButton.Click += (s, e) =>
            {
                TimeSpan time = new TimeSpan
                (
                    days: 0,
                    hours: 0,
                    minutes: this.MinuteNumberPicker.Index,
                    seconds: this.SecondNumberPicker.Index,
                    milliseconds: this.MillisecondNumberPicker.Index * 10
                );
                this.Time = time > this.Maximum ? this.Maximum : time;

                this.MoveCommand?.Execute(this.MoveCommandParameter);
                this.Flyout.Hide();
            };
        }

        // FlowDirection
        private void ConstructFlowDirection()
        {
            bool isRightToLeft = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

            base.FlowDirection = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        // Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.MinutesTextBlock.Text = resource.GetString("Minutes");
            this.SecondsTextBlock.Text = resource.GetString("Seconds");
            this.MillisecondsTextBlock.Text = resource.GetString("Milliseconds");
        }

    }
}