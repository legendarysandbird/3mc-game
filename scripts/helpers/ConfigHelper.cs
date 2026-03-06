using Godot;

public static class ConfigHelper
{
    private const string INPUT_SETTINGS_PATH = "user://input_settings.cfg";
    private const string MOUSE_AND_KEYBOARD_DISABLED_SETTING = "mouse_and_keyboard_disabled";

    private static bool? _mouseAndKeyboardDisabled;

    public static void SaveValue(string path, string key, Godot.Variant value, string section = "")
    {
        var config = new ConfigFile();
        config.Load(path);

        config.SetValue(section, key, value);
        config.Save(path);

        GD.Print($"Setting {key} to {value}!");
    }

    public static Variant GetValue(string path, string key, Variant defaultValue, string section = "")
    {
        var config = new ConfigFile();
        var error = config.Load(path);
        if (error != Error.Ok)
        {
            return defaultValue;
        }

        return (bool)config.GetValue(section, key);
    }

    public static void SetMouseAndKeyboardDisabled(bool disabled)
    {
        ConfigHelper.SaveValue(INPUT_SETTINGS_PATH, MOUSE_AND_KEYBOARD_DISABLED_SETTING, disabled);
        _mouseAndKeyboardDisabled = disabled;
    }

    public static bool GetMouseAndKeyboardDisabled()
    {
        if (_mouseAndKeyboardDisabled.HasValue)
        {
            return _mouseAndKeyboardDisabled.Value;
        }

        return (bool)ConfigHelper.GetValue(INPUT_SETTINGS_PATH, MOUSE_AND_KEYBOARD_DISABLED_SETTING, false);
    }
}
