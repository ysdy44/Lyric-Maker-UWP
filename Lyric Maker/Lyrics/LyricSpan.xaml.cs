using Windows.UI.Xaml.Documents;

namespace Lyric_Maker.Lyrics
{
    /// <summary>
    /// Span of Lyric.
    /// </summary>
    public sealed partial class LyricSpan : Span
    {
        public Run TimeRun => this._TimeRun;
        public Run TextRun => this._TextRun;

        //@Construct
        /// <summary>
        /// Initializes a LyricSpan. 
        /// </summary>
        public LyricSpan()
        {
            this.InitializeComponent();
        }
    }
}