using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

public static class NullHelpers
{
    public static T NotNull<T>([NotNull] this T? obj, string objName)
    {
        return obj ?? throw new ArgumentNullException(objName);
    }

    public static T GDNotNull<T>([NotNull] this T? node, string nodeName) where T : GodotObject
    {
        if (node == null || !GodotObject.IsInstanceValid(node))
        {
            throw new ArgumentNullException(nodeName);
        }

        return node;
    }
}
