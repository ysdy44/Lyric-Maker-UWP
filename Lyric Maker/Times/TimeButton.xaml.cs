using System;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.System;
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
        /// Initializes a Roulette. 
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
                this.MinuteRoulette.Index = this.Time.Minutes;
                this.SecondRoulette.Index = this.Time.Seconds;
                this.MillisecondRoulette.Index = this.Time.Milliseconds / 10;
            };

            this.CancelButton.Click += (s, e) => this.Flyout.Hide();
            this.OKButton.Click += (s, e) =>
            {
                TimeSpan time = new TimeSpan
                (
                    days: 0,
                    hours: 0,
                    minutes: this.MinuteRoulette.Index,
                    seconds: this.SecondRoulette.Index,
                    milliseconds: this.MillisecondRoulette.Index * 10
                );
                this.Time = time > this.Maximum ? this.Maximum : time;

                this.MoveCommand?.Execute(this.MoveCommandParameter);
                this.Flyout.Hide();
            };

            this.MinuteRoulette.ItemClick += (s, e) => this.ItemClick(this.MinuteTextBox);
            this.MinuteTextBox.GettingFocus += (s, e) => this.GettingFocus2(this.MinuteTextBox, this.MinuteRoulette);
            this.MinuteTextBox.LostFocus += (s, e) => this.LostFocus2(this.MinuteTextBox, this.MinuteRoulette);
            this.MinuteTextBox.KeyDown += (s, e) => this.KeyDown2(e.Key);

            this.SecondRoulette.ItemClick += (s, e) => this.ItemClick(this.SecondTextBox);
            this.SecondTextBox.GettingFocus += (s, e) => this.GettingFocus2(this.SecondTextBox, this.SecondRoulette);
            this.SecondTextBox.LostFocus += (s, e) => this.LostFocus2(this.SecondTextBox, this.SecondRoulette);
            this.SecondTextBox.KeyDown += (s, e) => this.KeyDown2(e.Key);

            this.MillisecondRoulette.ItemClick += (s, e) => this.ItemClick(this.MillisecondTextBox);
            this.MillisecondTextBox.GettingFocus += (s, e) => this.GettingFocus2(this.MillisecondTextBox, this.MillisecondRoulette);
            this.MillisecondTextBox.LostFocus += (s, e) => this.LostFocus2(this.MillisecondTextBox, this.MillisecondRoulette);
            this.MillisecondTextBox.KeyDown += (s, e) => this.KeyDown2(e.Key);
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


        private void ItemClick(TextBox textBox)
        {
            textBox.Visibility = Visibility.Visible;
            textBox.Focus(FocusState.Programmatic);
        }
        private void GettingFocus2(TextBox textBox, Roulette roulette)
        {
            textBox.Text = roulette.Index.ToString("D2");

            textBox.Visibility = Visibility.Visible;
            this.BodyGrid.IsHitTestVisible = false;

            textBox.SelectAll();
        }
        private void LostFocus2(TextBox textBox, Roulette roulette)
        {
            this.BodyGrid.IsHitTestVisible = true;
            textBox.Visibility = Visibility.Collapsed;

            string text = textBox.Text;
            if (int.TryParse(text, out int result))
            {
                roulette.Index = result;
            }
        }
        private void KeyDown2(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Execute:
                case VirtualKey.Enter:
                    this.OKButton.Focus(FocusState.Programmatic);
                    break;
                default:
                    break;
            }
        }

    }
}