using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Lyric_Maker.Times
{
    /// <summary>
    /// Represents a Tool with a revolving toothed wheel, 
    /// used to select the number you want.
    /// </summary>
    public sealed partial class Roulette : UserControl
    {

        //@Delegate
        /// <summary> Occurs when the clicked selected item. </summary>
        public event EventHandler<int> ItemClick;

        //@Converter
        private int OffsetToIndexConverter(double value)
        {
            double verticalOffset = value - this.ItemHeight / 2;
            int index = (int)Math.Round(verticalOffset / this.ItemHeight);

            if (index < 0) return 0;
            if (index >= this.Count) return this.Count - 1;
            return index;
        }

        bool IsScrollViewer { get => this.ScrollViewer.IsEnabled; set => this.ScrollViewer.IsEnabled = value; }
        bool IsChangeView { get => this.StackPanel.IsHitTestVisible; set => this.StackPanel.IsHitTestVisible = value; }

        #region DependencyProperty


        public double ItemHeight
        {
            get => (double)base.GetValue(ItemHeightProperty);
            set => base.SetValue(ItemHeightProperty, value);
        }
        /// <summary> Identifies the <see cref="Roulette.ItemHeight"/> dependency property. </summary>
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(nameof(ItemHeight), typeof(double), typeof(Roulette), new PropertyMetadata(40d));


        public int Index
        {
            get => (int)base.GetValue(IndexProperty);
            set => base.SetValue(IndexProperty, value);
        }
        /// <summary> Identifies the <see cref="Roulette.Index"/> dependency property. </summary>
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(nameof(Index), typeof(int), typeof(Roulette), new PropertyMetadata(-1, (sender, e) =>
        {
            Roulette control = (Roulette)sender;

            if (e.NewValue is int value)
            {
                control.IndexChanged(value, false);

                for (int i = 0; i < control.StackPanel.Children.Count; i++)
                {
                    ContentPresenter item = control.StackPanel.Children[i] as ContentPresenter;

                    item.Foreground = i == value ? control.BorderBrush : control.Foreground;

                    switch (Math.Abs(i - value))
                    {
                        case 0: item.Opacity = 1; break;
                        case 1: item.Opacity = 0.8; break;
                        case 2: item.Opacity = 0.5; break;
                        default: item.Opacity = 0.2; break;
                    }
                }
            }
        }));

        internal void IndexChanged(int value, bool disableAnimation)
        {
            this.ChangeView(value, this.IsChangeView, disableAnimation);

            for (int i = 0; i < this.StackPanel.Children.Count; i++)
            {
                ContentPresenter item = this.StackPanel.Children[i] as ContentPresenter;

                item.Foreground = i == value ? this.BorderBrush : this.Foreground;

                switch (Math.Abs(i - value))
                {
                    case 0: item.Opacity = 1; break;
                    case 1: item.Opacity = 0.8; break;
                    case 2: item.Opacity = 0.5; break;
                    default: item.Opacity = 0.2; break;
                }
            }
        }


        public int Count
        {
            get => (int)base.GetValue(CountProperty);
            set => base.SetValue(CountProperty, value);
        }
        /// <summary> Identifies the <see cref="Roulette.Count"/> dependency property. </summary>
        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(nameof(Count), typeof(int), typeof(Roulette), new PropertyMetadata(10, (sender, e) =>
        {
            Roulette control = (Roulette)sender;
            if (e.NewValue == e.OldValue) return;

            if (e.OldValue is int oldValue)
            {
                foreach (ContentPresenter item in control.StackPanel.Children)
                {
                    item.Tapped -= control.Item_Tapped;
                }
                control.StackPanel.Children.Clear();
            }

            if (e.NewValue is int value)
            {
                for (int i = 0; i < value; i++)
                {
                    ContentPresenter item = new ContentPresenter
                    {
                        Content = i.ToString("D2"),
                        Tag = i
                    };
                    item.Tapped += control.Item_Tapped;
                    control.StackPanel.Children.Add(item);
                }
            }
        }));


        #endregion


        //@Construct
        /// <summary>
        /// Initializes a Roulette. 
        /// </summary>
        public Roulette()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) => this.ChangeView(this.Index, true, true);
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                if (e.NewSize.Height == e.PreviousSize.Height) return;

                double height = e.NewSize.Height;
                this.HeaderBorder.Height = height / 2;
                this.FooterBorder.Height = height / 2;

                this.ChangeView(this.Index, true, true);
            };


            this.DispatcherTimer.Tick += (s, e) =>
            {
                this.DispatcherTimer.Stop();
                this.IsChangeView = true;

                this.ChangeView(this.Index, true, false);
            };
            this.HeaderBorder.Tapped += (s, e) => this.Index = 0;
            this.FooterBorder.Tapped += (s, e) => this.Index = this.Count - 1;


            this.ScrollViewer.DirectManipulationStarted += (s, e) =>
            {
                this.DispatcherTimer.Stop();
                this.IsChangeView = false;
                this.IsScrollViewer = false;
            };
            this.ScrollViewer.DirectManipulationCompleted += (s, e) =>
            {
                this.DispatcherTimer.Stop();
                this.IsChangeView = true;
                this.IsScrollViewer = true;

                this.Index = this.OffsetToIndexConverter(this.ScrollViewer.VerticalOffset);
            };


            this.ScrollViewer.ViewChanging += (s, e) =>
            {
                this.IsChangeView = false;

                this.Index = this.OffsetToIndexConverter(e.NextView.VerticalOffset);
            };
            this.ScrollViewer.ViewChanged += (s, e) =>
            {
                if (this.IsScrollViewer is false) return;

                this.DispatcherTimer.Stop();
                this.DispatcherTimer.Start();
            };
        }

        private void ChangeView(int value, bool isChangeView, bool disableAnimation)
        {
            if (isChangeView is false) return;

            double verticalOffset = value * this.ItemHeight + this.ItemHeight / 2;
            this.ScrollViewer.ChangeView(null, verticalOffset, null, disableAnimation);
        }

        private void Item_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is ContentPresenter item)
            {
                if (item.Tag is int index)
                {
                    if (this.Index == index)
                        this.ItemClick?.Invoke(this, index);//Delegate
                    else
                        this.Index = index;
                }
            }
        }

    }
}