using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

public static class NullHelpers
{
    public static T NotNull<T>([NotNull] this T? node, string objName)
    {
        if (node == null || (node is GodotObject gdObject && !GodotObject.IsInstanceValid(gdObject)))
        {
            throw new ArgumentNullException(objName);
        }

        return node;
    }
}
