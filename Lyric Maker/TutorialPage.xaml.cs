using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Lyric_Maker
{
    /// <summary> 
    /// Represents a page used to tutorial.
    /// </summary>
    public sealed partial class TutorialPage : Page
    {
        //@Construct
        /// <summary>
        /// Initializes a TutorialPage. 
        /// </summary>
        public TutorialPage()
        {
            this.InitializeComponent();
            this.ConstructFlowDirection();
            this.ConstructStrings();

            this.Forward00Button.Click += (s, e) => this.FlipView.SelectedIndex = 1;
            this.Forward02Button.Click += (s, e) => this.FlipView.SelectedIndex = 2;
            this.Forward03Button.Click += (s, e) => this.FlipView.SelectedIndex = 3;

            this.Button.Click += (s, e) =>
            {
                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
                else
                {
                    base.Frame.Navigate(typeof(MainPage));
                }
            };
        }

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

            this.Tutorial00TextBlock.Text = resource.GetString("Tutorial00"); 
            this.Tutorial01TextBlock.Text = resource.GetString("Tutorial01");
            this.Tutorial02TextBlock.Text = resource.GetString("Tutorial02"); 
            this.Tutorial03TextBlock.Text = resource.GetString("Tutorial03"); 
            this.Tutorial04TextBlock.Text = resource.GetString("Tutorial04"); 
            this.Button.Content = resource.GetString("Tutorial05"); 


            this.InputText00Run.Text =
            this.InputText01Run.Text =
            this.InputText02Run.Text =

            this.InputText10Run.Text =
            this.InputText11Run.Text =
            this.InputText12Run.Text =

            this.InputTextTextBlock.Text =
            resource.GetString("$InputText");

            this.TitleRun.Text = resource.GetString("Title");
            this.ArtistRun.Text = resource.GetString("Artist");
            this.AlbumRun.Text = resource.GetString("Album");
            this.LyricsEditorRun.Text = resource.GetString("LyricsEditor");
            this.TimeOffsetRun.Text = resource.GetString("TimeOffset");

            this.AddTextBlock.Text = resource.GetString("Add");
            this.PlayTextBlock.Text = resource.GetString("Play");
            this.PauseTextBlock.Text = resource.GetString("Pause");
            this.PasteTextBlock.Text = resource.GetString("Paste");
            this.CopyTextBlock.Text = resource.GetString("Copy");
            this.DuplicateTextBlock.Text = resource.GetString("Duplicate");
            this.RemoveTextBlock.Text = resource.GetString("Remove");
        }

    }
}