using Lyric_Maker.Lyrics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Media.Playback;
using Windows.Storage;
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
        private double LyricsToScaleYConverter(IList<Lyric> value) => value.Count == 0 ? 1 : 32 * value.Count / value.Sum(l => l.Line.Height);
        private string LyricsToSubtitleConverter(IList<Lyric> value, TimeSpan position) => value.Count == 0 ? string.Empty : value.LastOrDefault(l => l.Time <= position)?.Text ?? string.Empty;


        #region DependencyProperty


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

                control.SubtitleRun.Text = control.LyricsToSubtitleConverter(control.ObservableCollection, value);

                foreach (Lyric item in control.ObservableCollection)
                {
                    item.Position = value;
                }
            }
        }));

        /// <summary> Gets or sets <see cref = "MainPage" />'s duration. </summary>
        public TimeSpan Duration
        {
            get => (TimeSpan)base.GetValue(DurationProperty);
            set => base.SetValue(DurationProperty, value);
        }
        /// <summary> Identifies the <see cref = "MainPage.Duration" /> dependency property. </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(nameof(Duration), typeof(TimeSpan), typeof(MainPage), new PropertyMetadata(TimeSpan.FromMinutes(10), (sender, e) =>
        {
            MainPage control = (MainPage)sender;

            if (e.NewValue is TimeSpan value)
            {
                foreach (Lyric item in control.ObservableCollection)
                {
                    item.Duration = value;
                }            
            }
        }));


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
            this.ConstructFlowDirection();
            this.ConstructStrings();


            // Extend TitleBar
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;


            // Tick
            this.Timer.Tick += (s, e) =>
            {
                if (this.MediaPlayer.Source is null)
                    this.Position += this.SpeedInterval;
                else
                    this.Position = this.MediaPlayer.PlaybackSession.Position;

                if (this.Duration == TimeSpan.Zero || this.Position >= this.Duration)
                    this.IsPlaying = false;
            };


            // Drag and Drop 
            base.AllowDrop = true;
            base.Drop += async (s, e) =>
            {
                base.IsEnabled = false;
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                    foreach (IStorageItem item in items)
                    {
                        if (item is StorageFile file)
                        {
                            bool isLyric = this.IsLyric(file);
                            if (isLyric) { await this.Open(file); break; }

                            bool isMusic = this.IsMusic(file);
                            if (isMusic) { await this.OpenMusic(file); break; }
                        }
                    }
                }
                base.IsEnabled = true;
            };
            base.DragOver += (s, e) =>
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                //e.DragUIOverride.Caption = 
                e.DragUIOverride.IsCaptionVisible = e.DragUIOverride.IsContentVisible = e.DragUIOverride.IsGlyphVisible = true;
            };


            // Size
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                if (e.NewSize.Height == e.PreviousSize.Height) return;

                double height = e.NewSize.Height;
                this.Row01.MaxHeight = height - this.Row01.MinHeight - this.Row02.MinHeight - this.Row03.MinHeight - 2;
            };

            this.ControlScrollViewer.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                if (e.NewSize.Height == e.PreviousSize.Height) return;

                double height = e.NewSize.Height;
                double heightHalf = height / 2;
                this.ControlCanvas.Margin = new Thickness(0, heightHalf, 0, heightHalf);
            };

            this.LineCanvas.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                if (e.NewSize.Width == e.PreviousSize.Width) return;

                foreach (Lyric item in this.ObservableCollection)
                {
                    item.Length = e.NewSize.Width;
                }
            };


            // Pane
            this.PaneListView.ItemClick += async (s, e) =>
            {
                if (e.ClickedItem is StackPanel item &&
                    item.Children.FirstOrDefault() is Border border &&
                    border.Child is SymbolIcon symbolIcon)
                {
                    this.IsPlaying = false;
                    base.IsEnabled = false;
                    switch (symbolIcon.Symbol)
                    {
                        case Symbol.Add: this.New(); break;
                        case Symbol.OpenFile: await this.Open(); break;
                        case Symbol.Save: await this.Save(); break;
                        case Symbol.Help: base.Frame.Navigate(typeof(TutorialPage)); break;
                        case Symbol.Important: await new AboutDialog().ShowAsync(); break;
                        case Symbol.Setting: break;
                    }
                    base.IsEnabled = true;
                }
            };

            // Speed
            foreach (object item in this.SpeedListView.Items)
            {
                if (item is RadioButton radioButton)
                {
                    radioButton.Click += (s, e) =>
                    {
                        if (radioButton.Content is double value)
                        {
                            this.MediaPlayer.PlaybackSession.PlaybackRate = value;
                            this.SpeedInterval = TimeSpan.FromMilliseconds(100 * value);
                        }
                    };
                }
            }


            // SplitButton
            this.PhoneSplitButton.Click += (s, e) => this.SplitView.IsPaneOpen = true;
            this.PCSplitButton.Click += (s, e) =>
            {
                switch (this.SplitView.DisplayMode)
                {
                    case SplitViewDisplayMode.Overlay:
                        this.SplitView.IsPaneOpen = false;
                        break;
                    case SplitViewDisplayMode.CompactOverlay:
                    case SplitViewDisplayMode.CompactInline:
                        this.SplitView.IsPaneOpen = !this.SplitView.IsPaneOpen;
                        break;
                }
            };


            // TextBox
            this.TitleTextBox.KeyDown += (s, e) => tabSelect(e.Key, this.ArtistTextBox);
            this.ArtistTextBox.KeyDown += (s, e) => tabSelect(e.Key, this.AlbumTextBox);
            this.AlbumTextBox.KeyDown += (s, e) => tabSelect(e.Key, this.LyricsEditorTextBox);
            this.LyricsEditorTextBox.KeyDown += (s, e) => tabSelect(e.Key, this.TitleTextBox);
            void tabSelect(VirtualKey key, TextBox nextTextBox)
            {
                if (key == VirtualKey.Enter)
                {
                    nextTextBox.Focus(FocusState.Programmatic);
                    nextTextBox.SelectAll();
                }
            }


            // AppBar
            this.AddButton.Click += (s, e) => this.Add();
            this.MoveCommand.Click += (s, item) => this.Move(item);
            this.PlayCommand.Click += (s, item) =>
            {
                TimeSpan time = item.Time;
                this.Position = time;
                this.MediaPlayer.PlaybackSession.Position = time;

                if (this.IsPlaying == false) this.IsPlaying = true;
            };
            this.PauseCommand.Click += (s, item) =>
            {
                TimeSpan time = item.Time;
                this.Position = time;
                this.MediaPlayer.PlaybackSession.Position = time;

                if (this.IsPlaying == true) this.IsPlaying = false;
            };
            this.PasteCommand.Click += (s, item) => this.Paste(item);
            this.CopyCommand.Click += (s, item) => this.Copy(item);
            this.DuplicateCommand.Click += (s, item) => this.Duplicate(item);
            this.RemoveCommand.Click += (s, item) => this.Remove(item);


            // Zoom
            this.ZoomListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is SymbolIcon item)
                {
                    this.EditerPivot.SelectedIndex = 1;
                    switch (item.Symbol)
                    {
                        case Symbol.Zoom: this.Zoom(); break;
                        case Symbol.ZoomIn: this.ZoomIn(); break;
                        case Symbol.ZoomOut: this.ZoomOut(); break;
                        default: break;
                    }
                }
            };


            // Music
            this.PlayMusicButton.Click += (s, e) =>
            {
                if (this.Position >= this.Duration)
                    this.Position = TimeSpan.Zero;
                this.IsPlaying = true;
            };
            this.PauseMusicButton.Click += (s, e) => this.IsPlaying = false;
            this.OpenMusicButton.Click += async (s, e) =>
            {
                this.IsPlaying = false;
                base.IsEnabled = false;
                await this.OpenMusic();
                base.IsEnabled = true;
                this.IsPlaying = true;
            };


            // Position
            this.ControlScrollViewer.DirectManipulationStarted += (s, e) => { if (this.IsPlaying) this.IsPlayingCore = false; };
            this.ControlScrollViewer.DirectManipulationCompleted += (s, e) => { if (this.IsPlaying) this.IsPlayingCore = true; };


            this.PositionSlider.ValueChangedStarted += (s, e) =>
            {
                if (this.IsPlaying) this.IsPlayingCore = false;

                TimeSpan position = this.DoubleToTimeSpanConverter(this.PositionSlider.Value);
                this.MediaPlayer.PlaybackSession.Position = position;
                this.Position = position;
            };
            this.PositionSlider.ValueChangedDelta += (s, e) =>
            {
                TimeSpan position = this.DoubleToTimeSpanConverter(e.NewValue);
                this.MediaPlayer.PlaybackSession.Position = position;
                this.Position = position;
            };
            this.PositionSlider.ValueChangedCompleted += (s, e) =>
            {
                if (this.IsPlaying) this.IsPlayingCore = true;
            };
        }
    }
}