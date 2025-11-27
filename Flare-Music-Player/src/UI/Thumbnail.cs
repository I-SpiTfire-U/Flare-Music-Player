using System.Diagnostics;

namespace Flare_Music_Player.src.UI;

public class Thumbnail(int x, int y, int width, int height)
{
    private string? _ThumbnailPath;
    private Process? _ImageProcess;
    private readonly bool _HasImageSupport = CheckForImageSupport();

    private int _XPosition = x;
    private int _YPosition = y;
    private int _Width = width;
    private int _Height = height;

    private bool _ThumbnailHasChanged = true;

    public void SetSize(int width, int height)
    {
        if (_Width == width && _Height == height)
        {
            return;
        }

        _Width = width;
        _Height = height;
        _ThumbnailHasChanged = true;
    }

    public void SetPosition(int x, int y)
    {
        if (_XPosition == x && _YPosition == y)
        {
            return;
        }

        _XPosition = x;
        _YPosition = y;
        _ThumbnailHasChanged = true;
    }

    public void SetThumbnailPath(string? thumbnailPath)
    {
        if (_ThumbnailPath == thumbnailPath)
        {
            return;
        }

        if (_ThumbnailPath != null && File.Exists(_ThumbnailPath))
        {
            try
            {
                File.Delete(_ThumbnailPath);
            }
            catch { }
        }

        _ThumbnailPath = thumbnailPath;
        _ThumbnailHasChanged = true;
    }

    public void Draw(bool force = false)
    {
        if (!_HasImageSupport || string.IsNullOrEmpty(_ThumbnailPath) || (!_ThumbnailHasChanged && !force))
        {
            return;
        }

        _ThumbnailHasChanged = false;

        if (_ImageProcess is not null && !_ImageProcess.HasExited)
        {
            _ImageProcess.Kill();
            _ImageProcess.WaitForExit();
            _ImageProcess.Dispose();
        }

        ProcessStartInfo psi = new()
        {
            FileName = "kitten",
            Arguments = $"icat --place {_Width}x{_Height}@{_XPosition}x{_YPosition} \"{_ThumbnailPath}\"",
            RedirectStandardOutput = false,
            UseShellExecute = true,
            CreateNoWindow = true
        };

        _ImageProcess = Process.Start(psi);
    }

    private static bool CheckForImageSupport()
    {
        string term = Environment.GetEnvironmentVariable("TERM") ?? "";
        string? kitty = Environment.GetEnvironmentVariable("KITTY_WINDOW_ID");

        if (kitty != null || term.Contains("kitty"))
        {
            return true;
        }

        return false;
    }
}