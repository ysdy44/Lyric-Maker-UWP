using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Lyric_Maker
{
    /// <summary> 
    /// Represents a dialog used to about....
    /// </summary>
    public sealed partial class AboutDialog : ContentDialog
    {
        //@Construct
        /// <summary>
        /// Initializes a AboutDialog. 
        /// </summary>
        public AboutDialog()
        {
            this.InitializeComponent();
            this.ConstructStrings();

            base.SecondaryButtonClick += (s, e) => base.Hide();
            base.PrimaryButtonClick += (s, e) => base.Hide();
        }

        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            base.SecondaryButtonText = resource.GetString("Cancel");
            base.PrimaryButtonText = resource.GetString("OK");

            this.VersionTextBlock.Text = resource.GetString("$Version");

            this.GithubTextBlock.Text = resource.GetString("Github");
            string githubLink = resource.GetString("$GithubLink");
            this.GithubHyperlinkButton.Content = githubLink;
            this.GithubHyperlinkButton.NavigateUri = new Uri(githubLink);

            this.FeedbackTextBlock.Text = resource.GetString("Feedback");
            string feedbackLink = resource.GetString("$FeedbackLink");
            this.FeedbackHyperlinkButton.Content = feedbackLink;
            this.FeedbackHyperlinkButton.NavigateUri = new Uri("mailto:" + feedbackLink);
        }

    }
}