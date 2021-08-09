using Lyric_Maker.Lyrics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Lyric_Maker.Controls
{
    /// <summary>
    /// Canvas for <see cref="Lyric.Control"/>s.
    /// </summary>
    public sealed class ControlCanvas : Canvas
    {

        //@Converter
        private double TimeSpanToDoubleConverter(TimeSpan value) => value.TotalSeconds;


        #region DependencyProperty


        /// <summary> Gets or sets the source of<see cref = "ControlCanvas" />'s items. </summary>
        public object ItemSource
        {
            get => (object)base.GetValue(IsPlayingProperty);
            set => base.SetValue(IsPlayingProperty, value);
        }
        /// <summary> Identifies the <see cref = "ControlCanvas.ItemSource" /> dependency property. </summary>
        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(nameof(ItemSource), typeof(object), typeof(ControlCanvas), new PropertyMetadata(false, (sender, e) =>
        {
            ControlCanvas control = (ControlCanvas)sender;

            if (e.NewValue is object value)
            {
                if ((control.ItemSourceNotify is null) is false)
                {
                    control.ItemSourceNotify.CollectionChanged -= control.ItemSourceNotify_CollectionChanged;
                    if (control.ItemSourceNotify is IEnumerable<Lyric> items)
                    {
                        foreach (Lyric item in items)
                        {
                            control.Children.Remove(item.Control);
                        }
                    }
                }
                control.ItemSourceNotify = value as INotifyCollectionChanged;
                if ((control.ItemSourceNotify is null) is false)
                {
                    control.ItemSourceNotify.CollectionChanged += control.ItemSourceNotify_CollectionChanged;
                    if (control.ItemSourceNotify is IEnumerable<Lyric> items)
                    {
                        foreach (Lyric item in items)
                        {
                            control.Children.Add(item.Control);
                        }
                    }
                }
            }
        }));


        /// <summary> Gets or sets <see cref = "ControlCanvas" />'s duration. </summary>
        public TimeSpan Duration
        {
            get => (TimeSpan)base.GetValue(DurationProperty);
            set => base.SetValue(DurationProperty, value);
        }
        /// <summary> Identifies the <see cref = "ControlCanvas.Duration" /> dependency property. </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(nameof(Duration), typeof(TimeSpan), typeof(ControlCanvas), new PropertyMetadata(TimeSpan.FromMinutes(10), (sender, e) =>
        {
            ControlCanvas control = (ControlCanvas)sender;

            if (e.NewValue is TimeSpan value)
            {
                control.UpdateHeight(value, control.Scale);
                control.UpdateLines(control.ActualWidth, control.ActualHeight, control.Scale);
            }
        }));


        /// <summary> Gets or sets <see cref = "ControlCanvas" />'s scale. </summary>
        public double Scale
        {
            get => (double)base.GetValue(ScaleProperty);
            set => base.SetValue(ScaleProperty, value);
        }
        /// <summary> Identifies the <see cref = "ControlCanvas.Scale" /> dependency property. </summary>
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(ControlCanvas), new PropertyMetadata(16d, (sender, e) =>
        {
            ControlCanvas control = (ControlCanvas)sender;

            if (e.NewValue is double value)
            {
                control.UpdateHeight(control.Duration, value);
                control.UpdateLines(control.ActualWidth, control.ActualHeight, value);
            }
        }));


        #endregion


        private readonly IList<Rectangle> CanvasLines = new List<Rectangle>();
        private INotifyCollectionChanged ItemSourceNotify;


        //@Construct
        /// <summary>
        /// Initializes a ControlCanvas. 
        /// </summary>
        public ControlCanvas()
        {
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;

                if (e.NewSize.Width != e.PreviousSize.Width)
                {
                    foreach (Rectangle item in this.CanvasLines)
                    {
                        if (Canvas.GetZIndex(item) == -1 || Canvas.GetZIndex(item) == 1)
                        {
                            item.Width = e.NewSize.Width;
                        }
                    }
                }

                if (e.NewSize.Height != e.PreviousSize.Height)
                {
                    this.UpdateLines(e.NewSize.Width, e.NewSize.Height, this.Scale);
                }
            };
            base.Loaded += (s, e) =>
            {
                this.UpdateHeight(this.Duration, this.Scale);
                this.UpdateLines(base.ActualWidth, base.ActualHeight, this.Scale);
            };

        }

        private void UpdateHeight(TimeSpan duration, double scale)
        {
            double duration2 = this.TimeSpanToDoubleConverter(duration);
            this.Height = duration2 * scale;
        }
        private void UpdateLines(double width, double lenght, double scale)
        {
            while (scale < 25)
                scale *= 5;

            while (scale > 125)
                scale /= 5;

            foreach (Rectangle item in this.CanvasLines)
            {
                base.Children.Remove(item);
            }
            this.CanvasLines.Clear();

            double space = scale;
            for (double i = 0; i < lenght; i += space)
            {
                Rectangle item = new Rectangle
                {
                    Width = width,
                    Height = 2,
                    Opacity = 0.3,
                    Fill = new SolidColorBrush(Colors.Gray)
                };
                Canvas.SetTop(item, i);
                Canvas.SetZIndex(item, -1);

                this.CanvasLines.Add(item);
                base.Children.Add(item);
            }
            double space2 = 5 * scale;
            for (double i = 0; i < lenght; i += space2)
            {
                Rectangle item = new Rectangle
                {
                    Width = width,
                    Height = 1,
                    Fill = new SolidColorBrush(Colors.Gray)
                };
                Canvas.SetTop(item, i);
                Canvas.SetZIndex(item, -1);

                this.CanvasLines.Add(item);
                base.Children.Add(item);
            }
        }

        private void ItemSourceNotify_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems[0] is Lyric itemAdd)
                    {
                        int index = e.NewStartingIndex;
                        base.Children.Insert(index, itemAdd.Control);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    {
                        int index = e.OldStartingIndex;
                        base.Children.RemoveAt(index);
                    }
                    if (e.NewItems[0] is Lyric itemMove)
                    {
                        int index = e.NewStartingIndex;
                        base.Children.Insert(index, itemMove.Control);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        int index = e.OldStartingIndex;
                        base.Children.RemoveAt(index);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        int index = e.OldStartingIndex;
                        base.Children.RemoveAt(index);
                    }
                    if (e.NewItems[0] is Lyric itemReplace)
                    {
                        int index = e.NewStartingIndex;
                        base.Children.Insert(index, itemReplace.Control);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    while (true)
                    {
                        UIElement remove = base.Children.LastOrDefault(t => Canvas.GetZIndex(t) == 0);
                        if (remove is null) break;
                        else base.Children.Remove(remove);
                    }
                    break;

                default:
                    break;
            };
        }

    }
}