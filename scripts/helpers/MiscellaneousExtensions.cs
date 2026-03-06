using Godot;

public static class MiscellaneousExtensions
{
    public static bool IsRoughlyZero(this float num)
    {
        return Mathf.Abs(num) <= Mathf.Epsilon;
    }

    public static bool RoughlyEquals(this float num1, float num2)
    {
        return IsRoughlyZero(num2 - num1);
    }
}
