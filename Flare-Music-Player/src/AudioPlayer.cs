using LibVLCSharp.Shared;
using TagLib;
using Timer = System.Timers.Timer;

namespace Flare_Music_Player.src;

public class AudioPlayer
{
    private readonly LibVLC _LibVLC;
    private readonly MediaPlayer _MediaPlayer;
    private readonly Timer _UpdateTimer;

    private long _DurationMs = 0;

    public int Volume { get; private set; } = 50;
    public Action? OnUpdate;
    public Action? OnTrackEnded;

    public long DurationMs => _DurationMs;
    public TimeSpan Duration => TimeSpan.FromMilliseconds(_DurationMs);

    public long CurrentTimeMs => _MediaPlayer.Time;
    public TimeSpan CurrentTime => TimeSpan.FromMilliseconds(CurrentTimeMs);

    public bool IsPlaying => _MediaPlayer.IsPlaying;
    public bool IsPaused => _MediaPlayer.State == VLCState.Paused;

    public string? CurrentArtworkPath { get; private set; }

    public AudioPlayer()
    {
        Core.Initialize();

        _LibVLC = new(enableDebugLogs: false, "--no-video", "--no-skip-frames");
        _MediaPlayer = new(_LibVLC)
        {
            Volume = Volume
        };

        _UpdateTimer = new(500)
        {
            AutoReset = true
        };
        _UpdateTimer.Elapsed += (_, _) => OnUpdate?.Invoke();
        _MediaPlayer.LengthChanged += (_, e) =>
        {
            _DurationMs = e.Length;
            OnUpdate?.Invoke();
        };

        _MediaPlayer.EndReached += (_, _) =>
        {
            OnTrackEnded?.Invoke();
        };
    }

    public void SetTrack(string filePath)
    {
        Stop();
        _UpdateTimer.Stop();

        _MediaPlayer.Media = null;
        _MediaPlayer.Media = new(_LibVLC, filePath, FromType.FromPath);

        CurrentArtworkPath = ExtractAlbumArt(filePath);
    }

    public void Play()
    {
        if (_MediaPlayer.Media == null)
        {
            return;
        }

        _MediaPlayer.Volume = Volume;

        _MediaPlayer.SetVideoTrack(-1);

        _MediaPlayer.Play();
        _UpdateTimer.Start();
    }

    public void Pause()
    {
        _MediaPlayer.Pause();
    }

    public void Stop()
    {
        _MediaPlayer.Stop();
        _UpdateTimer.Stop();
    }

    private void SetVolume(int volume)
    {
        Volume = Math.Clamp(volume, 0, 100);
        _MediaPlayer.Volume = Volume;
    }

    public void IncreaseVolume(int amount)
    {
        SetVolume(Volume + amount);
    }

    public void DecreaseVolume(int amount)
    {
        SetVolume(Volume - amount);
    }

    public void Seek(long ms)
    {
        if (DurationMs <= 0)
        {
            return;
        }

        _MediaPlayer.Time = ms < DurationMs
            ? Math.Clamp(ms, 0, DurationMs)
            : DurationMs - 1;
    }

    public void SeekSeconds(int seconds)
    {
        if (_MediaPlayer.Media is null)
        {
            return;
        }

        long newMs = CurrentTimeMs + seconds * 1000;
        Seek(newMs);

        if (!IsPlaying)
        {
            Play();
        }
    }

    public static string? ExtractAlbumArt(string filePath)
    {
        try
        {
            TagLib.File file = TagLib.File.Create(filePath);
            IPicture[] pictures = file.Tag.Pictures;
            if (pictures.Length > 0)
            {
                IPicture pic = pictures[0];
                string ext = pic.MimeType switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    _ => ".bin"
                };

                string tmpFile = Path.Combine(Path.GetTempPath(), $"flare_art_{Guid.NewGuid()}{ext}");
                System.IO.File.WriteAllBytes(tmpFile, pic.Data.Data);
                return tmpFile;
            }
        }
        catch {}
        
        return null;
    }
}