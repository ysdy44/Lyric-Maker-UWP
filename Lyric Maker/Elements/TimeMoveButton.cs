using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Lyric_Maker.Elements
{
    /// <summary>
    /// Represents a Button, that is used to Move to change the Time.
    /// </summary>
    public class TimeMoveButton : Button
    {

        //@Converter
        private double TimeSpanToDoubleConverter(TimeSpan value) => value.TotalSeconds;
        private TimeSpan DoubleToTimeSpanConverter(double value) => TimeSpan.FromSeconds(value);


        #region DependencyProperty


        public double SpliterWidth
        {
            get => (double)base.GetValue(SpliterWidthProperty);
            set => base.SetValue(SpliterWidthProperty, value);
        }
        /// <summary> Identifies the <see cref="TimeMoveButton.SpliterWidth"/> dependency property. </summary>
        public static readonly DependencyProperty SpliterWidthProperty = DependencyProperty.Register(nameof(SpliterWidth), typeof(double), typeof(TimeMoveButton), new PropertyMetadata(4d));

        public TimeSpan Time
        {
            get => (TimeSpan)base.GetValue(TimeProperty);
            set => base.SetValue(TimeProperty, value);
        }
        /// <summary> Identifies the <see cref="TimeMoveButton.Time"/> dependency property. </summary>
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(nameof(Time), typeof(TimeSpan), typeof(TimeMoveButton), new PropertyMetadata(TimeSpan.Zero));

        public double Scale
        {
            get => (double)base.GetValue(ScaleProperty);
            set => base.SetValue(ScaleProperty, value);
        }
        /// <summary> Identifies the <see cref="TimeMoveButton.Scale"/> dependency property. </summary>
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(TimeMoveButton), new PropertyMetadata(16.0));


        public object MoveCommandParameter
        {
            get => (object)base.GetValue(MoveCommandParameterProperty);
            set => base.SetValue(MoveCommandParameterProperty, value);
        }
        /// <summary> Identifies the <see cref="TimeMoveButton.MoveCommandParameter"/> dependency property. </summary>
        public static readonly DependencyProperty MoveCommandParameterProperty = DependencyProperty.Register(nameof(MoveCommandParameter), typeof(object), typeof(TimeMoveButton), new PropertyMetadata(null));


        public ICommand MoveCommand
        {
            get => (ICommand)base.GetValue(MoveCommandProperty);
            set => base.SetValue(MoveCommandProperty, value);
        }
        /// <summary> Identifies the <see cref="TimeMoveButton.MoveCommand"/> dependency property. </summary>
        public static readonly DependencyProperty MoveCommandProperty = DependencyProperty.Register(nameof(MoveCommand), typeof(ICommand), typeof(TimeMoveButton), new PropertyMetadata(null));


        #endregion

        double startingSpliterWidth = 4;
        double startingTime = 0;
        FlyoutBase startingFlyout = null;


        //@Construct
        /// <summary>
        /// Initializes a TimeMoveButton. 
        /// </summary>
        public TimeMoveButton()
        {
            base.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            base.ManipulationStarted += (s, e) =>
            {
                this.startingSpliterWidth = this.SpliterWidth;
                this.startingTime = this.TimeSpanToDoubleConverter(this.Time) * this.Scale;
                this.startingFlyout = base.Flyout;
                base.Flyout = null;
            };
            base.ManipulationDelta += (s, e) =>
            {
                this.startingSpliterWidth += e.Delta.Translation.X;
                this.SpliterWidth = this.startingSpliterWidth < 4 ? 4 : this.startingSpliterWidth;
                this.startingTime += e.Delta.Translation.Y;
                this.Time = this.DoubleToTimeSpanConverter(this.startingTime < 0 ? 0 : this.startingTime / this.Scale);
            };
            base.ManipulationCompleted += async (s, e) =>
            {        
                this.MoveCommand?.Execute(this.MoveCommandParameter);

                await Task.Delay(100);
                base.Flyout = this.startingFlyout;
            };
        }
    }
}