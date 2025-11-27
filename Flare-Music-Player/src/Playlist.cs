namespace Flare_Music_Player.src;

public class Playlist
{
    public int ActiveSongIndex { get; private set; }
    private readonly string _Directory;
    private readonly string[] _Songs;

    public int Count => _Songs.Length;
    public IEnumerable<string> SongNames => _Songs.Select(f => Path.GetFileNameWithoutExtension(f));

    public Playlist(string directory, int startingSong = 0)
    {
        _Directory = directory;
        _Songs = Directory.GetFiles(_Directory);
        if (_Songs.Length == 0)
        {
            throw new InvalidOperationException("No songs found in directory.");
        }
        ActiveSongIndex = Math.Clamp(startingSong, 0, _Songs.Length - 1);
    }

    public void SetActiveSong(int index)
    {
        if (index < 0 || index > _Songs.Length - 1)
        {
            return;
        }
        ActiveSongIndex = index;
    }

    public void ShiftActiveSong(int amount)
    {
        ActiveSongIndex = (ActiveSongIndex + amount + _Songs.Length) % _Songs.Length;
    }

    public string GetActiveSong()
    {
        return _Songs[ActiveSongIndex];
    }

    public string? GetSongNameAtIndex(int index)
    {
        return (uint)index < (uint)_Songs.Length
            ? Path.GetFileNameWithoutExtension(_Songs[index])
            : null;
    }

    public string GetActiveSongName()
    {
        return Path.GetFileNameWithoutExtension(_Songs[ActiveSongIndex]);
    }
}