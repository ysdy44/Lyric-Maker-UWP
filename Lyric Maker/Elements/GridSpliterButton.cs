using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Lyric_Maker.Elements
{
    /// <summary>
    /// Represents a Button that can be dragged to change the height property
    /// </summary>
    public class GridSpliterButton : Button
    {


        #region DependencyProperty


        /// <summary> Gets or set the value for <see cref="GridSpliterButton"/>. </summary>
        public GridLength Value
        {
            get => (GridLength)base.GetValue(ValueProperty);
            set => base.SetValue(ValueProperty, value);
        }
        /// <summary> Identifies the <see cref="GridSpliterButton.Value"/> dependency property. </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(GridLength), typeof(GridSpliterButton), new PropertyMetadata(new GridLength(276.0)));


        /// <summary> Gets or set the orientation for <see cref="GridSpliterButton"/>. </summary>
        public Orientation Orientation
        {
            get => this.orientation;
            set
            {
                switch (value)
                {
                    case Orientation.Vertical:
                        base.ManipulationMode = ManipulationModes.TranslateY;
                        break;
                    case Orientation.Horizontal:
                        base.ManipulationMode = ManipulationModes.TranslateX;
                        break;
                    default:
                        base.ManipulationMode = ManipulationModes.TranslateY;
                        break;
                }
                this.orientation = value;
            }
        }
        private Orientation orientation;


        #endregion

        double startingValue = 276;

        //@Construct
        /// <summary>
        /// Initializes a GridSpliterButton. 
        /// </summary>
        public GridSpliterButton()
        {
            base.Loaded += (s, e) => this.Orientation = this.Orientation;
            base.ManipulationStarted += (s, e) => this.startingValue = this.Value.Value;
            base.ManipulationDelta += (s, e) =>
            {
                switch (this.Orientation)
                {
                    case Orientation.Vertical:
                        this.startingValue += e.Delta.Translation.Y;
                        break;
                    case Orientation.Horizontal:
                        this.startingValue += e.Delta.Translation.X;
                        break;
                }
                this.Value = new GridLength(this.startingValue < 0 ? 0 : this.startingValue);
            };
            base.ManipulationCompleted += (s, e) => { };
        }
    }
}