using System.Diagnostics;
using System.Timers;
using NAudio.Wave;

namespace Flare_Music_Player;

public class AudioPlayer
{
    private bool _TrackIsPaused = false;
    private WaveOutEvent? _OutputDevice;
    private AudioFileReader? _AudioFile;
    public float Volume { get; private set; } = 0.5f;

    private readonly Stopwatch _ElapsedTime;
    private readonly System.Timers.Timer _UpdateTimer;

    private TimeSpan _SongDuration = TimeSpan.Zero;
    public Action? OnUpdateTimerElapsed;
    public Action? OnPlaybackStopped;

    public AudioPlayer()
    {
        _ElapsedTime = new();
        _UpdateTimer = new(1000)
        {
            AutoReset = true
        };
        _UpdateTimer.Elapsed += UpdateTimerElapsed_Event;
    }

    private void UpdateTimerElapsed_Event(object? sender, ElapsedEventArgs  e)
    {
        OnUpdateTimerElapsed?.Invoke();
    }

    private void PlaybackStopped_Event(object? sender, EventArgs e)
    {
        OnPlaybackStopped?.Invoke();
    }

    public bool TrackHasEnded => _OutputDevice?.PlaybackState == PlaybackState.Stopped;

    public void SetTrack(string filePath)
    {
        Stop();

        _AudioFile = new AudioFileReader(filePath);
        _SongDuration = _AudioFile.TotalTime;
        _OutputDevice = new WaveOutEvent();
        _OutputDevice.Init(_AudioFile);
        _OutputDevice.PlaybackStopped += PlaybackStopped_Event;
    }

    public void Play()
    {
        if (_OutputDevice == null || _AudioFile == null)
        {
            return;
        }
        SetVolume(Volume);

        _OutputDevice.Play();
        _UpdateTimer.Start();
        _ElapsedTime.Start();
    }

    public void Stop()
    {
        _OutputDevice?.Stop();
        _OutputDevice?.Dispose();
        _OutputDevice = null;

        _AudioFile?.Dispose();
        _AudioFile = null;

        _SongDuration = TimeSpan.Zero;
        _ElapsedTime.Reset();
        _UpdateTimer.Stop();
    }

    public void Pause()
    {
        _TrackIsPaused = !_TrackIsPaused;
        if (_TrackIsPaused)
        {
            _OutputDevice?.Pause();
            _ElapsedTime.Stop();
            _UpdateTimer.Stop();
        }
        else
        {
            _OutputDevice?.Play();
            _ElapsedTime.Start();
            _UpdateTimer.Start();
        }
    }

    private void SetVolume(float newVolume)
    {
        Volume = Math.Clamp(newVolume, 0f, 1f);
        if (_AudioFile != null)
        {
            _AudioFile.Volume = Volume;
        }
    }

    public void IncreaseVolume(int amount)
    {
        float volume = Volume + (float)(amount * 0.01);
        SetVolume(volume);
    }

    public void DecreaseVolume(float amount)
    {
        float volume = Volume - (float)(amount * 0.01);
        SetVolume(volume);
    }

    public TimeSpan ElapsedTime => _ElapsedTime.Elapsed;
    public TimeSpan SongDuration => _SongDuration;
}