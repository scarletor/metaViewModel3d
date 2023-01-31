using static ModelDatas;

public class GameContext
{
    // Scene Passed Params
    public static MetaVerseData SelectedMetaverseData;

    // For Multiplay Game
    public static string RoomName;

    // For Character
    public static string UserName = "Player";
    public static bool IsMaleAvatar = true;
    public static int AvatarIndex = 0;
    public static RPMAvatar RPMAvatar = null;

    // For Ebook
    public static string EbookUrl;
}
