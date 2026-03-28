using Godot;

public static class Logger
{
    public static void Info(params object[] message)
    {
        GD.Print(message);
    }

    public static void Warn(params object[] message)
    {
        GD.PushWarning(message);
    }

    public static void Error(params object[] message)
    {
        GD.PushError(message);
    }
}
