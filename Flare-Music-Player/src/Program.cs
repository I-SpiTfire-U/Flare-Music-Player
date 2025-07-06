namespace Flare_Music_Player;

public class Program
{
    public static async Task Main(string[] args)
    {
        FlareMusicPlayer app = new(args);
        await app.Begin();
    }
}