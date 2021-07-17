using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Lyric_Maker.Lyrics
{
    /// <summary>
    /// Represents a Lyric that contains time, text, control information.
    /// </summary>
    public sealed class Lyric : INotifyPropertyChanged
    {

        //@Converter
        public string TimeSpanToStringConverter(TimeSpan value) => value.ToString("mm':'ss'.'ff");
        private double TimeSpanToDoubleConverter(TimeSpan value) => value.TotalSeconds;
        private TimeSpan DoubleToTimeSpanConverter(double value) => TimeSpan.FromSeconds(value);



        public LyricSpan Span { get; } = new LyricSpan();
        public Rectangle Line { get; } = new Rectangle
        {
            Width = 1,
            Height = 46,
            Fill = new SolidColorBrush(Colors.White)
        };
        public ContentControl Control { get; }


        public Lyric(DataTemplate dataTemplate)
        {
            this.Control = new ContentControl
            {
                Content = this,
                ContentTemplate = dataTemplate
            };
        }


        /// <summary> Gets or sets the length. </summary>
        public double Length
        {
            get => this.length;
            set
            {
                this.length = value;
                this.OnPropertyChanged(nameof(Length)); // Notify 

                Canvas.SetLeft(this.Line, this.Time.TotalMilliseconds / this.Duration.TotalMilliseconds * value);
            }
        }
        private double length = 512;


        /// <summary> Gets or sets the spliter width. </summary>
        public double SpliterWidth
        {
            get => this.spliterWidth;
            set
            {
                if (this.spliterWidth == value) return;
                this.spliterWidth = value;
                this.OnPropertyChanged(nameof(SpliterWidth)); // Notify 
            }
        }
        private double spliterWidth = 4;


        /// <summary> Gets or sets the time. </summary>
        public TimeSpan Time
        {
            get => this.time;
            set
            {
                this.time = value;
                this.OnPropertyChanged(nameof(Time)); // Notify 

                this.Span.TimeRun.Text = this.TimeSpanToStringConverter(value);
                Canvas.SetLeft(this.Line, value.TotalMilliseconds / this.Duration.TotalMilliseconds * this.Length);
                Canvas.SetTop(this.Control, this.TimeSpanToDoubleConverter(value) * this.Scale - 20);
            }
        }
        private TimeSpan time = TimeSpan.Zero;


        /// <summary> Gets or sets the duration. </summary>
        public TimeSpan Duration
        {
            get => this.duration;
            set
            {
                this.duration = value;
                this.OnPropertyChanged(nameof(Duration)); // Notify 

                Canvas.SetLeft(this.Line, this.Time.TotalMilliseconds / value.TotalMilliseconds * this.Length);
            }
        }
        private TimeSpan duration = TimeSpan.FromMinutes(10);


        /// <summary> Gets or sets the scale. </summary>
        public double Scale
        {
            get => this.scale;
            set
            {
                this.scale = value;
                this.OnPropertyChanged(nameof(Scale)); // Notify 

                Canvas.SetTop(this.Control, this.TimeSpanToDoubleConverter(this.Time) * value - 20);
            }
        }
        private double scale = 16;


        /// <summary> Gets or sets the text. </summary>
        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                this.OnPropertyChanged(nameof(Text)); // Notify 

                this.Line.Height = string.IsNullOrEmpty(value) ? 2 : 2 + value.Length * 2;
                this.Span.TextRun.Text = value;
            }
        }
        private string text;


        /// <summary> Gets or sets the state. </summary>
        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                if (this.isSelected == value) return;
                this.isSelected = value;
                this.OnPropertyChanged(nameof(IsSelected)); // Notify
            }
        }
        private bool isSelected;


        //@Notify 
        /// <summary> Multicast event for property change notifications. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName"> Name of the property used to notify listeners. </param>
        protected void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}