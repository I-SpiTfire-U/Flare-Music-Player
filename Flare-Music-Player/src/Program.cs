namespace Flare_Music_Player.src;

public class Program
{
    public static async Task Main(string[] args)
    {
        string? musicPath;
        if (args.Length == 0)
        {
            Console.Write("Music Path\n> ");
            musicPath = Console.ReadLine();
        }
        else
        {
            musicPath = args[0];
        }

        if (string.IsNullOrEmpty(musicPath) || !Path.Exists(musicPath))
        {
            Console.WriteLine($"Invalid Music Path: {musicPath}");
            return;
        }

        FlareMusicPlayer app = new(musicPath);
        await app.Begin();
    }
}