namespace Flare_Music_Player;

public class FlareMusicPlayer
{
    private readonly string _MusicDirectory;
    private readonly string[] _Songs;
    private int _CurrentTrack = 0;

    private readonly AudioPlayer _AudioPlayer;
    private int _LastWindowWidth;
    private int _LastWindowHeight;

    public FlareMusicPlayer(string[] args)
    {
        _LastWindowWidth = Console.WindowWidth;
        _LastWindowHeight = Console.WindowHeight;

        _MusicDirectory = args[0];
        _Songs = Directory.GetFiles(_MusicDirectory);
        for (int i = 0; i < _Songs.Length; i++)
        {
            _Songs[i] = Path.GetFileNameWithoutExtension(_Songs[i]);
        }

        _AudioPlayer = new();
        _AudioPlayer.OnUpdateTimerElapsed += UpdateTimerElapsed;
    }

    public async Task Begin()
    {
        while (true)
        {
            Console.CursorVisible = false;
            Console.Clear();
            await ReadUserInputLoopAsync();
        }
    }

    private async Task ReadUserInputLoopAsync()
    {
        _AudioPlayer.SetTrack(GetFullSongPath(_Songs[_CurrentTrack]));
        DisplayAllInfo();

        _AudioPlayer.Play();

        while (true)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = await Task.Run(() => Console.ReadKey(true));

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Q:
                        Environment.Exit(0);
                        break;
                    case ConsoleKey.Spacebar:
                        _AudioPlayer.Pause();
                        break;
                    case ConsoleKey.UpArrow:
                        _AudioPlayer.IncreaseVolume(1);
                        WriteAtPosition(VolumeBar(_AudioPlayer.Volume), 0, 4);
                        break;
                    case ConsoleKey.DownArrow:
                        _AudioPlayer.DecreaseVolume(1);
                        WriteAtPosition(VolumeBar(_AudioPlayer.Volume), 0, 4);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (_CurrentTrack > 0)
                        {
                            _AudioPlayer.Stop();
                            _CurrentTrack--;
                            return;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (_CurrentTrack < _Songs.Length - 1)
                        {
                            _AudioPlayer.Stop();
                            _CurrentTrack++;
                            return;
                        }
                        break;
                }
            }

            CheckIfWindowChanged();

            if (_AudioPlayer.TrackHasEnded)
            {
                _AudioPlayer.Stop();
                _CurrentTrack++;
                break;
            }
        }
    }

    private void CheckIfWindowChanged()
    {
        if (_LastWindowWidth != Console.WindowWidth || _LastWindowHeight != Console.WindowHeight)
        {
            _LastWindowWidth = Console.WindowWidth;
            _LastWindowHeight = Console.WindowHeight;
            
            Console.Clear();
            DisplayAllInfo();
        }
    }

    private void DisplayAllInfo()
    {
        Console.WriteLine("Now Playing:");
        Console.WriteLine($"{Path.GetFileNameWithoutExtension(_Songs[_CurrentTrack])}\n");
        Console.WriteLine(DurationBar(_AudioPlayer.ElapsedTime, _AudioPlayer.SongDuration));
        Console.WriteLine(VolumeBar(_AudioPlayer.Volume));
        
        Console.Write("\nNext Up:\n  ");
        Console.WriteLine(Path.GetFileNameWithoutExtension(_CurrentTrack < _Songs.Length - 1 ? _Songs[_CurrentTrack + 1] : "[End of Queue]"));
        Console.Write("Last Played:\n  ");
        Console.WriteLine(Path.GetFileNameWithoutExtension(_CurrentTrack > 0 ? _Songs[_CurrentTrack - 1] : "[End of Queue]"));
    }

    private string GetFullSongPath(string song)
    {
        return Path.Combine(_MusicDirectory, $"{song}.mp3");
    }

    private void UpdateTimerElapsed()
    {
        WriteAtPosition(DurationBar(_AudioPlayer.ElapsedTime, _AudioPlayer.SongDuration), 0, 3);
    }

    private static void WriteAtPosition(string text, int x, int y)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(text);
    }

    private static string DurationBar(TimeSpan elapsed, TimeSpan duration)
    {
        if (duration.TotalSeconds == 0)
        {
            return "[--------------------]";
        }
        int totalBars = 20;
        double progress = elapsed.TotalSeconds / duration.TotalSeconds;
        int bars = (int)(progress * totalBars);

        return $"{elapsed:mm\\:ss} [{new string('■', bars).PadRight(20, '-')}] {duration:mm\\:ss}";
    }

    private static string VolumeBar(float volume)
    {
        int bars = (int)(volume * 20);
        return $"  vol [{new string('■', bars).PadRight(20, '-')}] {volume:P0} ";
    }
}