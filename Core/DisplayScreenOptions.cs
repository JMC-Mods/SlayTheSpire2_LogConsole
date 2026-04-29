using Godot;

namespace JmcLogConsole.Core;

public static class DisplayScreenOptions
{
    public const string FollowGameWindow = "跟随游戏窗口";
    public const string PrimaryScreen = "主显示器";

    public static IReadOnlyList<string> GetOptions()
    {
        var options = new List<string>
        {
            FollowGameWindow,
            PrimaryScreen
        };

        int screenCount = GetScreenCount();
        int primaryScreen = GetPrimaryScreen();
        for (int screen = 0; screen < screenCount; screen++)
        {
            options.Add(FormatScreenOption(screen, primaryScreen));
        }

        DisplayDiagnostics.LogDisplaySnapshot("Build default-open-screen dropdown options: " + string.Join(" | ", options));

        return options;
    }

    public static string NormalizeOption(string? option)
    {
        if (string.IsNullOrWhiteSpace(option))
        {
            return FollowGameWindow;
        }

        if (string.Equals(option, FollowGameWindow, StringComparison.Ordinal)
            || string.Equals(option, PrimaryScreen, StringComparison.Ordinal))
        {
            return option;
        }

        return TryParseScreenIndex(option, out int screen)
            ? FormatScreenOption(screen, GetPrimaryScreen())
            : FollowGameWindow;
    }

    public static bool TryParseScreenIndex(string? option, out int screen)
    {
        screen = -1;
        if (string.IsNullOrWhiteSpace(option))
        {
            return false;
        }

        return TryParseNumberAfterPrefix(option, "显示器 ", zeroBased: false, out screen)
            || TryParseNumberAfterPrefix(option, "Display ", zeroBased: false, out screen)
            || TryParseMonitorIndex(option, out screen);
    }

    private static string FormatScreenOption(int screen, int primaryScreen)
    {
        return screen == primaryScreen
            ? $"显示器 {screen + 1}（主显示器）"
            : $"显示器 {screen + 1}";
    }

    private static bool TryParseNumberAfterPrefix(string option, string prefix, bool zeroBased, out int screen)
    {
        screen = -1;
        if (!option.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        int start = prefix.Length;
        int end = start;
        while (end < option.Length && char.IsDigit(option[end]))
        {
            end++;
        }

        if (end == start || !int.TryParse(option[start..end], out int number))
        {
            return false;
        }

        screen = zeroBased ? number : number - 1;
        return screen >= 0;
    }

    private static bool TryParseMonitorIndex(string option, out int screen)
    {
        screen = -1;
        const string prefix = "Monitor (";
        if (!option.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        int closeIndex = option.IndexOf(')', prefix.Length);
        return closeIndex > prefix.Length
            && int.TryParse(option[prefix.Length..closeIndex], out screen)
            && screen >= 0;
    }

    private static int GetScreenCount()
    {
        try
        {
            return DisplayServer.GetScreenCount();
        }
        catch
        {
            return 0;
        }
    }

    private static int GetPrimaryScreen()
    {
        try
        {
            return DisplayServer.GetPrimaryScreen();
        }
        catch
        {
            return 0;
        }
    }

}
