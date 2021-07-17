using System;
using System.Collections.Generic;

namespace Lyric_Maker.TimeTexts
{
    /// <summary>
    /// Data of Lyric.
    /// </summary>
    public struct LyricData
    {
        //@Converter
        private static int CharToIntConverter(char value)
        {
            switch (value)
            {
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
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
            string tag2 = $"[{tag}:";
            foreach (string line in lines)
            {
                // ti
                if (line.Contains(tag) == false) continue;
                // [ti:
                if (line.Contains(tag2) == false) continue;

                // [ti:Music]
                string[] split = line.Split('[', ':', ']');
                if (split.Length != 4) continue;

                // ti
                string tag3 = split[1].Trim();
                // Music
                string mete = split[2].Trim();

                return mete;
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
                if (string.IsNullOrEmpty(line)) continue;

                // [00:00.00] Text
                string[] split = line.Split('[', ']');
                if (split.Length != 3) continue;

                // 00:00.00
                string time = split[1].Trim();
                // Text
                string text = split[2].Trim();

                // 00:00
                if (time.Length == 5) yield return new LyricData
                {
                    Text = text,
                    Time = new TimeSpan
                    (
                         days: 0,
                         hours: 0,
                         minutes: 10 * LyricData.CharToIntConverter(time[0]) + LyricData.CharToIntConverter(time[1]),
                         seconds: 10 * LyricData.CharToIntConverter(time[3]) + LyricData.CharToIntConverter(time[4])
                    )
                };

                // 00:00.00
                else if (time.Length == 8) yield return new LyricData
                {
                    Text = text,
                    Time = new TimeSpan
                    (
                         days: 0,
                         hours: 0,
                         minutes: 10 * LyricData.CharToIntConverter(time[0]) + LyricData.CharToIntConverter(time[1]),
                         seconds: 10 * LyricData.CharToIntConverter(time[3]) + LyricData.CharToIntConverter(time[4]),
                         milliseconds: 100 * LyricData.CharToIntConverter(time[6]) + 10 * LyricData.CharToIntConverter(time[7])
                    )
                };
            }
        }

    }
}