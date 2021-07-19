using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lyric_Maker.Lyrics
{
    /// <summary>
    /// Data of Lyric.
    /// </summary>
    public struct LyricData
    {
        //@Static
        private static readonly Regex LyricRegex = new Regex(@"\[(?<minutes>\d{2}).(?<seconds>\d{2}).(?<milliseconds>\d{1,3})\]\s?(?<content>.*)");
        private static readonly Regex TagRegex = new Regex(@"\[.*:.*\]");
        private static Regex GetTagRegex(string tag) => new Regex($@"\[{tag}:(.*)\]");
        private static int StringToInt(string milliseconds)
        {
            int msLength = milliseconds.Length;
            int msMete = int.Parse(milliseconds);
            switch (msLength)
            {
                case 1: return msMete * 100;
                case 2: return msMete * 10;
                case 3: return msMete;
                default: return 0;
            }
        }


        /// <summary> Gets or sets the text. </summary>
        public string Text;
        /// <summary> Gets or sets the time. </summary>
        public TimeSpan Time;

        /// <summary>
        /// Gets mete of lrc file.
        /// </summary>
        /// <param name="lines"> The all string lines. </param>
        /// <param name="tag"> The mete tag. </param>
        public static string GetMete(IEnumerable<string> lines, string tag)
        {
            Regex regex = LyricData.GetTagRegex(tag);
            foreach (string line in lines)
            {
                string input = line.ToLower();
                if (LyricData.TagRegex.IsMatch(line))
                {
                    Match titleMatch = regex.Match(input);
                    if (titleMatch.Success)
                    {
                        return titleMatch.Groups[1].Value;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets datas of lrc file.
        /// </summary>
        /// <param name="lines"> The all string lines. </param>
        public static IEnumerable<LyricData> CreateDatas(IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                Match match = LyricData.LyricRegex.Match(line);
                if (match.Success)
                {
                    yield return new LyricData
                    {
                        Text = match.Groups["content"].Value,
                        Time = new TimeSpan
                        (
                            days: 0,
                            hours: 0,
                            minutes: int.Parse(match.Groups["minutes"].Value),
                            seconds: int.Parse(match.Groups["seconds"].Value),
                            milliseconds: LyricData.StringToInt(match.Groups["milliseconds"].Value)
                        )
                    };
                }
            }
        }

    }
}