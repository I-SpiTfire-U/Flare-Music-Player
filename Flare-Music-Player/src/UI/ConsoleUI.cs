namespace Flare_Music_Player.src.UI;

public static class ConsoleUI
{
    private static string _EmptyLine = string.Empty;
    private static readonly object _Lock = new();

    public static void SetClearLineLength()
    {
        _EmptyLine = new(' ', Console.BufferWidth - 1);
    }

    public static void ClearLine(int left, int top)
    {
        WriteAtPosition(left, top, _EmptyLine);
    }

    public static void WriteAtPosition(int left, int top, string text)
    {
        lock (_Lock)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(text);
        }
    }

    public static void ClearAndWriteAtPosition(int left, int top, string text)
    {
        lock (_Lock)
        {
            ClearLine(left, top);
            WriteAtPosition(left, top, text);
        }
    }
}