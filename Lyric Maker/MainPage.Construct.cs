using Lyric_Maker.Lyrics;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Lyric_Maker
{
    /// <summary> 
    /// Represents a page used to editing lyrics.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private string Untitled = "Untitled";
        private string InputText = "Please input the text";


        // FlowDirection
        private void ConstructFlowDirection()
        {
            bool isRightToLeft = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

            base.FlowDirection = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        // Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.PCAppTitleTextBlock.Text = resource.GetString("$DisplayName");
            this.PadAppTitleTextBlock.Text = resource.GetString("$DisplayName");

            this.Untitled = resource.GetString("$Untitled");
            this.InputText = resource.GetString("$InputText");


            this.NewTextBlock.Text = resource.GetString("New");
            this.OpenTextBlock.Text = resource.GetString("Open");
            this.SaveTextBlock.Text = resource.GetString("Save");
            this.TutorialTextBlock.Text = resource.GetString("Tutorial");
            this.SettingTextBlock.Text = resource.GetString("Setting");
            this.SpeedListView.Header = resource.GetString("Speed");

            this.ZoomToolTip.Content = resource.GetString("Zoom");
            this.ZoomInToolTip.Content = resource.GetString("ZoomIn");
            this.ZoomOutToolTip.Content = resource.GetString("ZoomOut");

            this.SpliteToolTip.Content = resource.GetString("Splite");

            this.InfoTextBlock.Text = resource.GetString("Info");
            this.LyricsTextBlock.Text = resource.GetString("Lyrics");

            this.TitleTextBox.Header = resource.GetString("Title");
            this.ArtistTextBox.Header = resource.GetString("Artist");
            this.AlbumTextBox.Header = resource.GetString("Album");
            this.LyricsEditorTextBox.Header = resource.GetString("LyricsEditor");
            this.TimeOffsetSlider.Header = resource.GetString("TimeOffset");

            this.FindButton.Label = resource.GetString("Find");
            this.AddButton.Label = resource.GetString("Add");
            this.MoveButton.Label = resource.GetString("Move");
            this.PlayButton.Label = resource.GetString("Play");
            this.PauseButton.Label = resource.GetString("Pause");
            this.PasteButton.Label = resource.GetString("Paste");
            this.CopyButton.Label = resource.GetString("Copy");
            this.DuplicateButton.Label = resource.GetString("Duplicate");
            this.RemoveButton.Label = resource.GetString("Remove");

            this.PlayMusicToolTip.Content = resource.GetString("PlayMusic");
            this.PauseMusicToolTip.Content = resource.GetString("PauseMusic");
            this.OpenMusicToolTip.Content = resource.GetString("OpenMusic");
        }


        private void Zoom()
        {
            double scale = 16 / this.ControlCanvas.Scale;
            {
                double verticalOffset = this.ControlScrollViewer.VerticalOffset * scale;
                this.ControlCanvas.Scale = 16;
                foreach (Lyric item2 in this.ObservableCollection)
                {
                    item2.Scale = 16;
                }

                double position = (this.Line.Y1 + this.Line.Y2) / 2 * scale;
                this.Line.Y1 = this.Line.Y2 = position;

                bool disableAnimation = true;
                this.ControlScrollViewer.ChangeView(null, verticalOffset, null, disableAnimation);
            }
        }

        private void ZoomIn()
        {
            double verticalOffset = this.ControlScrollViewer.VerticalOffset * 1.1d;
            this.ControlCanvas.Scale *= 1.1f;
            foreach (Lyric item2 in this.ObservableCollection)
            {
                item2.Scale = this.ControlCanvas.Scale;
            }

            double position = (this.Line.Y1 + this.Line.Y2) / 2 * 1.1f;
            this.Line.Y1 = this.Line.Y2 = position;

            bool disableAnimation = true;
            this.ControlScrollViewer.ChangeView(null, verticalOffset, null, disableAnimation);
        }

        private void ZoomOut()
        {
            double verticalOffset = this.ControlScrollViewer.VerticalOffset / 1.1d;
            this.ControlCanvas.Scale /= 1.1f;
            foreach (Lyric item2 in this.ObservableCollection)
            {
                item2.Scale = this.ControlCanvas.Scale;
            }

            double position = (this.Line.Y1 + this.Line.Y2) / 2 / 1.1f;
            this.Line.Y1 = this.Line.Y2 = position;

            bool disableAnimation = true;
            this.ControlScrollViewer.ChangeView(null, verticalOffset, null, disableAnimation);
        }

    }
}