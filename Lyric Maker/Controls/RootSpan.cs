using Lyric_Maker.Lyrics;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace Lyric_Maker.Controls
{
    /// <summary>
    /// Root Span for <see cref="Lyric.Span"/>s.
    /// </summary>
    public sealed class RootSpan : Span
    {

        #region DependencyProperty


        /// <summary> Gets or sets the source of<see cref = "RootSpan" />'s items. </summary>
        public object ItemSource
        {
            get => (object)base.GetValue(IsPlayingProperty);
            set => base.SetValue(IsPlayingProperty, value);
        }
        /// <summary> Identifies the <see cref = "RootSpan.ItemSource" /> dependency property. </summary>
        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(nameof(ItemSource), typeof(object), typeof(RootSpan), new PropertyMetadata(false, (sender, e) =>
        {
            RootSpan control = (RootSpan)sender;

            if (e.NewValue is object value)
            {
                if ((control.ItemSourceNotify is null) == false) control.ItemSourceNotify.CollectionChanged -= control.ItemSourceNotify_CollectionChanged;
                control.ItemSourceNotify = value as INotifyCollectionChanged;
                if ((control.ItemSourceNotify is null) == false) control.ItemSourceNotify.CollectionChanged += control.ItemSourceNotify_CollectionChanged;
            }
        }));


        #endregion


        private INotifyCollectionChanged ItemSourceNotify;

        private void ItemSourceNotify_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems[0] is Lyric itemAdd)
                    {
                        int index = e.NewStartingIndex;
                        base.Inlines.Insert(index, itemAdd.Span);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    {
                        int index = e.OldStartingIndex;
                        base.Inlines.RemoveAt(index);
                    }
                    if (e.NewItems[0] is Lyric itemMove)
                    {
                        int index = e.NewStartingIndex;
                        base.Inlines.Insert(index, itemMove.Span);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        int index = e.OldStartingIndex;
                        base.Inlines.RemoveAt(index);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        int index = e.OldStartingIndex;
                        base.Inlines.RemoveAt(index);
                    }
                    if (e.NewItems[0] is Lyric itemReplace)
                    {
                        int index = e.NewStartingIndex;
                        base.Inlines.Insert(index, itemReplace.Span);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    base.Inlines.Clear();
                    break;

                default:
                    break;
            };
        }

    }
}