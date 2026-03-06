using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

public static class NullHelpers
{
    public static T NotNull<T>([NotNull] this T? obj, string objName)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(objName);
        }

        if (obj is GodotObject gdObject && !GodotObject.IsInstanceValid(gdObject))
        {
            throw new ArgumentException($"{objName} is not a valid instance!");
        }

        return obj;
    }

    public static T IsInitialized<T>([NotNull] this T? obj, string objName)
    {
        var def = default(T);
        if (def != null && def.Equals(obj))
        {
            throw new ArgumentException($"{objName} has not been initialized!");
        }

        return obj.NotNull(objName);
    }
}
