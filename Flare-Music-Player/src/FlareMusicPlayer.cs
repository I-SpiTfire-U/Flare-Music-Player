using Flare_Music_Player.src.UI;

namespace Flare_Music_Player.src;

public class FlareMusicPlayer
{
    private readonly Playlist _Playlist;

    private readonly AudioPlayer _AudioPlayer;
    private int _LastWindowWidth;
    private int _LastWindowHeight;

    private bool _TrackFinished = false;

    private readonly Thumbnail _AudioThumbnail = new(0, 9, 30, 30);
    private readonly InfoBar _DurationBar = new(0, 20);
    private readonly InfoBar _VolumeBar = new(0, 20);

    public FlareMusicPlayer(string musicPath)
    {
        ConsoleUI.SetClearLineLength();

        _LastWindowWidth = Console.WindowWidth;
        _LastWindowHeight = Console.WindowHeight;

        _Playlist = new(musicPath);

        _AudioPlayer = new();
        _AudioPlayer.OnUpdate += UpdateTimerElapsed;
        _AudioPlayer.OnTrackEnded += ()=> _TrackFinished = true;
    }

    public async Task Begin()
    {
        Console.Clear();

        while (true)
        {
            Console.CursorVisible = false;
            await ReadUserInputLoopAsync();
        }
    }

    private async Task ReadUserInputLoopAsync()
    {
        _AudioPlayer.SetTrack(_Playlist.GetActiveSong());
        _AudioPlayer.Play();

        _AudioThumbnail.SetThumbnailPath(_AudioPlayer.CurrentArtworkPath);
        _AudioThumbnail.Draw(force: true);

        DisplayStaticInfo();
        DisplayVolumeBar();

        while (true)
        {
            if (Console.KeyAvailable)
            {
                byte inputKeyChar = (byte)char.ToLower(Console.ReadKey(true).KeyChar);

                switch (inputKeyChar)
                {
                    case 113:
                        Console.Clear();
                        Environment.Exit(0);
                        break;
                    case 32:
                        _AudioPlayer.Pause();
                        break;
                    case 57:
                        _AudioPlayer.DecreaseVolume(1);
                        DisplayVolumeBar();
                        break;
                    case 48:
                        _AudioPlayer.IncreaseVolume(1);
                        DisplayVolumeBar();
                        break;
                    case 45:
                        _AudioPlayer.SeekSeconds(-5);
                        UpdateTimerElapsed();
                        break;
                    case 61:
                        _AudioPlayer.SeekSeconds(5);
                        UpdateTimerElapsed();
                        break;
                    case 91:
                        UpdateTrack(-1);
                        break;
                    case 93:
                        UpdateTrack(1);
                        break;
                }
            }

            if (_TrackFinished)
            {
                _TrackFinished = false;
                UpdateTrack(1);
            }

            CheckIfWindowChanged();
            UpdateTimerElapsed();

            await Task.Delay(50);
        }
    }

    private void UpdateTimerElapsed()
    {
        ConsoleUI.ClearAndWriteAtPosition(0, 8, GetDurationBar(_AudioPlayer.CurrentTime, _AudioPlayer.DurationMs));
    }

    private void UpdateTrack(int delta)
    {
        _Playlist.ShiftActiveSong(delta);

        _AudioPlayer.Stop();
        _AudioPlayer.SetTrack(_Playlist.GetActiveSong());
        _AudioPlayer.Play();

        _AudioThumbnail.SetThumbnailPath(_AudioPlayer.CurrentArtworkPath);
        _AudioThumbnail.Draw(force: true);

        Console.Clear();
        DisplayStaticInfo();
        DisplayVolumeBar();
    }

    private void CheckIfWindowChanged()
    {
        if (_LastWindowWidth == Console.WindowWidth && _LastWindowHeight == Console.WindowHeight)
        {
            return;
        }

        ConsoleUI.SetClearLineLength();

        _LastWindowWidth = Console.WindowWidth;
        _LastWindowHeight = Console.WindowHeight;

        Console.Clear();

        DisplayStaticInfo();
        _AudioThumbnail.Draw(force: true);
        DisplayVolumeBar();
    }

    private void DisplayStaticInfo()
    {   
        string? next = _Playlist.GetSongNameAtIndex(_Playlist.ActiveSongIndex + 1);
        string nextSong = next != null
            ? $"{Truncate(next, Console.WindowWidth)}"
            : "<End of Queue>";
        ConsoleUI.WriteAtPosition(0, 0, "::Next Song::");
        ConsoleUI.ClearAndWriteAtPosition(0, 1, nextSong);

        string? prev = _Playlist.GetSongNameAtIndex(_Playlist.ActiveSongIndex - 1);
        string prevSong = prev != null
            ? $"{Truncate(prev, Console.WindowWidth)}"
            : "<Start of Queue>";
        ConsoleUI.WriteAtPosition(0, 2, "::Previous Song::");
        ConsoleUI.ClearAndWriteAtPosition(0, 3, prevSong);

        string song = Truncate(_Playlist.GetActiveSongName(), Console.WindowWidth);
        ConsoleUI.WriteAtPosition(0, 5, "::Currently Playing::");
        ConsoleUI.ClearAndWriteAtPosition(0, 6, $"{song}");

        _AudioThumbnail.Draw();
    }

    public void DisplayVolumeBar()
    {
        ConsoleUI.ClearAndWriteAtPosition(0, 7, GetVolumeBar(_AudioPlayer.Volume));
    }

    public string GetDurationBar(TimeSpan elapsedTime, long durationMs)
    {
        double progress = durationMs > 0 ? elapsedTime.TotalSeconds / TimeSpan.FromMilliseconds(durationMs).TotalSeconds : 0;
        _DurationBar.SetProgress(progress);

        return $"{_DurationBar.GetBar} {elapsedTime:hh\\:mm\\:ss} / {TimeSpan.FromMilliseconds(durationMs):hh\\:mm\\:ss}";
    }

    public string GetVolumeBar(float volume)
    {
        double progress = volume / 100.0;
        _VolumeBar.SetProgress(progress);

        return $"{_VolumeBar.GetBar} Volume: {volume}%";
    }

    public static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) 
        {
            return "";
        }

        return text.Length > maxLength 
            ? text[..(maxLength - 4)] + "..." 
            : text;
    }
}
