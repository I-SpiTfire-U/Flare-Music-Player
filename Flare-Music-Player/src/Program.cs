namespace Flare_Music_Player.src;

public class Program
{
    public static async Task Main(string[] args)
    {
        FlareMusicPlayer app = new(args);
        await app.Begin();
    }
}