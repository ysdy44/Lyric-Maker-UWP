using Lyric_Maker.Lyrics;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Lyric_Maker
{
    /// <summary> 
    /// Represents a page used to editing lyrics.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //@Converter
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Visible : Visibility.Collapsed;
        private Visibility ReverseBooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;
        private Visibility IndexToVisibilityConverter(int value) => value == 0 ? Visibility.Collapsed : Visibility.Visible;
        private string TimeSpanToStringConverter(TimeSpan value) => value.ToString("mm':'ss'.'ff");
        private double TimeSpanToDoubleConverter(TimeSpan value) => value.TotalSeconds;
        private TimeSpan DoubleToTimeSpanConverter(double value) => TimeSpan.FromSeconds(value);
        private double LineYToCanvasTopConverter(double value) => value - 6;


        #region DependencyProperty


        /// <summary>
        /// Gets or sets the music name.
        /// </summary>
        public string MusicName
        {
            get => (string)base.GetValue(MusicNameProperty);
            set => base.SetValue(MusicNameProperty, value);
        }
        /// <summary> Identifies the <see cref="MainPage.MusicName"/> dependency property. </summary>
        public static readonly DependencyProperty MusicNameProperty = DependencyProperty.Register(nameof(MusicName), typeof(string), typeof(MainPage), new PropertyMetadata(null));


        /// <summary> Gets or sets <see cref = "MainPage" />'s position. </summary>
        public TimeSpan Position
        {
            get => (TimeSpan)base.GetValue(PositionProperty);
            set => base.SetValue(PositionProperty, value);
        }
        /// <summary> Identifies the <see cref = "MainPage.Position" /> dependency property. </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(TimeSpan), typeof(MainPage), new PropertyMetadata(TimeSpan.Zero, (sender, e) =>
        {
            MainPage control = (MainPage)sender;

            if (e.NewValue is TimeSpan value)
            {
                double position = control.TimeSpanToDoubleConverter(value);
                double position2 = position * control.ControlCanvas.Scale;
                control.Line.Y1 = control.Line.Y2 = position2;

                double verticalOffset = position2;
                bool disableAnimation = control.IsPlayingCore == false;
                control.ControlScrollViewer.ChangeView(null, verticalOffset, null, disableAnimation);
            }
        }));

        /// <summary> Gets or sets <see cref = "MainPage" />'s duration. </summary>
        public TimeSpan Duration
        {
            get => (TimeSpan)base.GetValue(DurationProperty);
            set => base.SetValue(DurationProperty, value);
        }
        /// <summary> Identifies the <see cref = "MainPage.Duration" /> dependency property. </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(nameof(Duration), typeof(TimeSpan), typeof(MainPage), new PropertyMetadata(TimeSpan.FromMinutes(10)));


        /// <summary> Gets or sets <see cref = "MainPage" />'s state of playing. </summary>
        public bool IsPlaying
        {
            get => (bool)base.GetValue(IsPlayingProperty);
            set => base.SetValue(IsPlayingProperty, value);
        }
        /// <summary> Identifies the <see cref = "MainPage.IsPlaying" /> dependency property. </summary>
        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(nameof(IsPlaying), typeof(bool), typeof(MainPage), new PropertyMetadata(false, (sender, e) =>
        {
            MainPage control = (MainPage)sender;

            if (e.NewValue is bool value)
            {
                control.IsPlayingCore = value;
            }
        }));


        #endregion


        /// <summary> Gets or sets the state of playback. </summary>
        public bool IsPlayingCore
        {
            get => this.isPlayingCore;
            set
            {
                if (value)
                {
                    this.MediaPlayer.Play();
                    this.Timer.Start();
                }
                else
                {
                    this.MediaPlayer.Pause();
                    this.Timer.Stop();
                }
                this.isPlayingCore = value;
            }
        }
        private bool isPlayingCore;

        private readonly ObservableCollection<Lyric> ObservableCollection = new ObservableCollection<Lyric>();
        private readonly MediaPlayer MediaPlayer = new MediaPlayer();

        private TimeSpan SpeedInterval = TimeSpan.FromMilliseconds(100);
        private readonly DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };


        //@Construct
        /// <summary>
        /// Initializes a MainPage. 
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }
    }
}