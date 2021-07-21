using Lyric_Maker.Lyrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace Lyric_Maker
{
    /// <summary> 
    /// Represents a page used to editing lyrics.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public void Add()
        {
            double verticalOffset = this.TimeSpanToDoubleConverter(this.Position) * this.ControlCanvas.Scale;
            this.ControlScrollViewer.ChangeView(null, verticalOffset, null);

            this.EditerPivot.SelectedIndex = 1;

            Lyric lyric = new Lyric(this.LyricTemplate)
            {
                Length = this.LineCanvas.Length,
                Duration = this.Duration,
                Time = this.Position,
                Text = this.InputText,
                Scale = this.ControlCanvas.Scale
            };

            int index = this.GetIndexAtPosition(this.ObservableCollection, this.Position);
            if (index == -1)
                this.ObservableCollection.Add(lyric);
            else
                this.ObservableCollection.Insert(index, lyric);
        }

        public void Move(Lyric item)
        {
            int startingIndex = this.ObservableCollection.IndexOf(item);
            int index = this.GetIndexAtPosition(this.ObservableCollection, item.Time);

            if (startingIndex == index - 1)
            {
                // None
            }
            else if (index == -1)
            {
                if (startingIndex == this.ObservableCollection.Count - 1)
                {
                    // None
                }
                else
                {
                    // To Last;
                    this.ObservableCollection.Move(startingIndex, this.ObservableCollection.Count - 1);
                }
            }
            else if (startingIndex > index)
            {
                // Move Up
                this.ObservableCollection.Move(startingIndex, index);
            }
            else if (startingIndex < index - 1)
            {
                // Move Down
                this.ObservableCollection.Move(startingIndex, index - 1);
            }
            else
            {
                // None
            }
        }

        public void Duplicate(Lyric item)
        {
            {
                int index = this.ObservableCollection.IndexOf(item);
                if (index < 0) return;
                if (index >= this.ObservableCollection.Count) return;
            }

            {
                int index = this.GetIndexAtPosition(this.ObservableCollection, item.Time);

                TimeSpan time;
                {
                    if (index == -1)
                        time = item.Time + TimeSpan.FromSeconds(1);
                    else
                    {
                        Lyric next = this.ObservableCollection[index];

                        if (next.Time - item.Time > TimeSpan.FromSeconds(2))
                            time = item.Time + TimeSpan.FromSeconds(1);
                        else
                        {
                            double seconds = item.Time.TotalSeconds + next.Time.TotalSeconds;
                            time = TimeSpan.FromSeconds(seconds / 2);
                        }
                    }
                }

                Lyric lyric = new Lyric(this.LyricTemplate)
                {
                    Length = this.LineCanvas.Length,
                    Duration = this.Duration,
                    Time = time,
                    Text = item.Text,
                    Scale = this.ControlCanvas.Scale
                };

                if (index == -1)
                    this.ObservableCollection.Add(lyric);
                else
                    this.ObservableCollection.Insert(index, lyric);
            }
        }

        public void Remove(Lyric item) => this.ObservableCollection.Remove(item);


        private int GetIndexAtPosition(IList<Lyric> lyrics, TimeSpan position)
        {
            int count = lyrics.Count;
            switch (count)
            {
                case 0: return -1;
                case 1:
                    Lyric single = lyrics.Single();
                    return (single.Time > position) ? 0 : -1;
                default:
                    for (int i = 0; i < count; i++)
                    {
                        Lyric lyric = lyrics[i];
                        if (lyric.Time > position)
                        {
                            return i;
                        }
                    }
                    return -1;
            }
        }


        public void New()
        {
            this.TitleTextBox.Text = string.Empty;
            this.ArtistTextBox.Text = string.Empty;
            this.AlbumTextBox.Text = string.Empty;
            this.LyricsEditorTextBox.Text = string.Empty;
            this.TimeOffsetSlider.Value = 0;
            this.ObservableCollection.Clear();
        }

        public bool IsLyric(StorageFile file)
        {
            string fileType = file.FileType.ToLower();
            switch (fileType)
            {
                case ".lrc":
                case ".txt":
                    return true;
                default:
                    return false;
            }
        }
        public async Task Open() => await this.Open(await new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.Desktop,
            FileTypeFilter =
            {
                ".lrc",
                ".txt"
            }
        }.PickSingleFileAsync());
        public async Task Open(StorageFile lyricFile)
        {
            if (lyricFile is null) return;

            IList<string> lines = await FileIO.ReadLinesAsync(lyricFile);
            if (lines is null) return;
            if (lines.Count <= 0) return;

            this.TitleTextBox.Text = LyricData.GetMete(lines, "ti");
            this.ArtistTextBox.Text = LyricData.GetMete(lines, "ar");
            this.AlbumTextBox.Text = LyricData.GetMete(lines, "al");
            this.LyricsEditorTextBox.Text = LyricData.GetMete(lines, "by");
            string offset = LyricData.GetMete(lines, "offset");
            if (int.TryParse(offset, out int result))
            {
                this.TimeOffsetSlider.Value = result;
            }

            IEnumerable<LyricData> datas = LyricData.CreateDatas(lines);
            if (datas is null) return;
            if (datas.Count() <= 0) return;

            this.Duration = datas.Max(t => t.Time) + TimeSpan.FromSeconds(5);
            double duration = this.TimeSpanToDoubleConverter(this.Duration);
            this.ControlCanvas.Height = duration * this.ControlCanvas.Scale;

            this.ObservableCollection.Clear();
            foreach (LyricData data in datas)
            {
                this.ObservableCollection.Add(new Lyric(this.LyricTemplate)
                {
                    Text = data.Text,
                    Time = data.Time,
                    Length = this.LineCanvas.Length,
                    Scale = this.ControlCanvas.Scale,
                    Duration = this.Duration
                });
            }
        }

        public async Task Save()
        {
            string suggested =
                string.IsNullOrEmpty(this.MusicName) == false ?
                this.MusicName :
                this.Untitled;

            StorageFile saveFile = await new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
                SuggestedFileName = suggested,
                FileTypeChoices =
                {
                    { "LRC", new[] { ".lrc" }},
                    { "TXT", new[] { ".txt" }}
                }
            }.PickSaveFileAsync();
            if (saveFile is null) return;

            await FileIO.WriteTextAsync(saveFile, this.PreviewTextBlock.Text);
        }


        public bool IsMusic(StorageFile file)
        {
            string fileType = file.FileType.ToLower();
            switch (fileType)
            {
                case ".mp3":
                case ".wmv":
                case ".m4a":
                case ".wav":
                case ".wma":
                    return true;
                default:
                    return false;
            }
        }
        public async Task OpenMusic() => await this.OpenMusic(await new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.MusicLibrary,
            ViewMode = PickerViewMode.Thumbnail,
            FileTypeFilter =
            {
                ".mp3",
                ".wmv",
                ".m4a",
                ".wav",
                ".wma"
            }
        }.PickSingleFileAsync());
        public async Task OpenMusic(StorageFile musicFile)
        {
            if (musicFile is null) return;

            MediaSource source = MediaSource.CreateFromStorageFile(musicFile);
            if (source is null) return;
            this.MediaPlayer.Source = source;
            this.MusicName = musicFile.DisplayName;
            this.Position = TimeSpan.Zero;

            MusicProperties music = await musicFile.Properties.GetMusicPropertiesAsync();
            if (string.IsNullOrEmpty(music.Title) == false) this.TitleTextBox.Text = music.Title;
            if (string.IsNullOrEmpty(music.Artist) == false) this.ArtistTextBox.Text = music.Artist;
            if (string.IsNullOrEmpty(music.Album) == false) this.AlbumTextBox.Text = music.Album;

            StorageItemThumbnail thumbnail = await musicFile.GetThumbnailAsync(ThumbnailMode.MusicView);
            switch (thumbnail.Type)
            {
                case ThumbnailType.Image:
                    this.BitmapImage.SetSource(thumbnail);
                    break;
                case ThumbnailType.Icon:
                    this.BitmapImage.UriSource = null;
                    break;
                default:
                    break;
            }

            while (true)
            {
                await Task.Delay(100);
                if (this.MediaPlayer.PlaybackSession.NaturalDuration != TimeSpan.Zero)
                {
                    this.Duration = source.Duration ?? new TimeSpan(10);
                    break;
                }
            }

            foreach (Lyric item in this.ObservableCollection)
            {
                item.Duration = this.Duration;
            }
        }

    }
}