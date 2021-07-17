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

        public GridLength SpliterHeight
        {
            get => (GridLength)base.GetValue(SpliterHeightProperty);
            set => base.SetValue(SpliterHeightProperty, value);
        }
        /// <summary> Identifies the <see cref="GridSpliterButton.SpliterHeight"/> dependency property. </summary>
        public static readonly DependencyProperty SpliterHeightProperty = DependencyProperty.Register(nameof(SpliterHeight), typeof(GridLength), typeof(GridSpliterButton), new PropertyMetadata(new GridLength(276.0)));

        #endregion

        double startingSpliterHeight = 276;


        //@Construct
        /// <summary>
        /// Initializes a GridSpliterButton. 
        /// </summary>
        public GridSpliterButton()
        {
            base.ManipulationMode = ManipulationModes.TranslateY;
            base.ManipulationStarted += (s, e) => this.startingSpliterHeight = this.SpliterHeight.Value;
            base.ManipulationDelta += (s, e) =>
            {
                this.startingSpliterHeight += e.Delta.Translation.Y;
                this.SpliterHeight = new GridLength(this.startingSpliterHeight < 0 ? 0 : this.startingSpliterHeight);
            };
            base.ManipulationCompleted += (s, e) => { };
        }
    }
}