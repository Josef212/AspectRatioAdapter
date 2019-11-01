using System;
using System.Reflection;
using UnityEngine;

public static class ScreenHelper
{
    private static MethodInfo s_getSizeOfMainGameViewMethod = null;

    public static bool IsTablet
    {
        get
        {
            Vector2 windowSize = GetMainGameViewSize();
            float aspectRatio = windowSize.x > windowSize.y ? windowSize.x / windowSize.y : windowSize.y / windowSize.x;
            return aspectRatio < 1.5f;
        }
    }

    public static string ResLog
    {
        get
        {
            Vector2 res = GetMainGameViewSize();
            return $"{res.x} x {res.y} - AR: {res.x / res.y} -> IsTablet: {IsTablet}";
        }
    }

    public static Vector2 GetMainGameViewSize()
    {
        if (s_getSizeOfMainGameViewMethod == null)
        {
            Type T = Type.GetType("UnityEditor.GameView,UnityEditor");
            s_getSizeOfMainGameViewMethod = T.GetMethod("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
        }

        return (Vector2)s_getSizeOfMainGameViewMethod.Invoke(null, null);
    }
}
