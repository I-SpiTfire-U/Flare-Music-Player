namespace Flare_Music_Player.src.UI;

public class InfoBar
{
    private readonly int _TotalBars;
    private double _Progress;

    private readonly char _FilledChar;
    private readonly char _EmptyChar;

    private string _InfoBarString = "";
    private readonly string _BlankBarString;

    public InfoBar(double progress, int totalBars, char filled = '■', char empty = '-')
    {
        _TotalBars = totalBars;
        _FilledChar = filled;
        _EmptyChar = empty;

        _BlankBarString = $"[{new string(_EmptyChar, _TotalBars)}]";
        SetProgress(progress);
    }

    public void SetProgress(double progress)
    {
        _Progress = Math.Clamp(progress, 0.0, 1.0);
        int bars = (int)(_Progress * _TotalBars);
        _InfoBarString = $"[{new string(_FilledChar, bars).PadRight(_TotalBars, _EmptyChar)}]";
    }

    public string GetBar => _InfoBarString;
    public string GetBlankBar => _BlankBarString;
}