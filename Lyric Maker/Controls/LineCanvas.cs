using Lyric_Maker.Lyrics;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Lyric_Maker.Controls
{
    /// <summary>
    /// Canvas for <see cref="Lyric.Line"/>s.
    /// </summary>
    public sealed class LineCanvas : Canvas
    {

        #region DependencyProperty


        /// <summary> Gets or sets the source of<see cref = "LineCanvas" />'s items. </summary>
        public object ItemSource
        {
            get => (object)base.GetValue(IsPlayingProperty);
            set => base.SetValue(IsPlayingProperty, value);
        }
        /// <summary> Identifies the <see cref = "LineCanvas.ItemSource" /> dependency property. </summary>
        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(nameof(ItemSource), typeof(object), typeof(LineCanvas), new PropertyMetadata(false, (sender, e) =>
        {
            LineCanvas control = (LineCanvas)sender;

            if (e.NewValue is object value)
            {
                if ((control.ItemSourceNotify is null) == false) control.ItemSourceNotify.CollectionChanged -= control.ItemSourceNotify_CollectionChanged;
                control.ItemSourceNotify = value as INotifyCollectionChanged;
                if ((control.ItemSourceNotify is null) == false) control.ItemSourceNotify.CollectionChanged += control.ItemSourceNotify_CollectionChanged;
            }
        }));


        #endregion

        public double Length => base.ActualWidth;
        private INotifyCollectionChanged ItemSourceNotify;

        private void ItemSourceNotify_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems[0] is Lyric itemAdd)
                    {
                        int index = e.NewStartingIndex;
                        base.Children.Insert(index, itemAdd.Line);
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
                        base.Children.Insert(index, itemMove.Line);
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
                        base.Children.Insert(index, itemReplace.Line);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    base.Children.Clear();
                    break;

                default:
                    break;
            };
        }

    }
}