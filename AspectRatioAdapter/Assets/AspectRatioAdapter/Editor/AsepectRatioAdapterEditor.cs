using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(AsepectRatioAdapter))]
public class AsepectRatioAdapterEditor : Editor
{
    private SerializedProperty m_script = null;
    private SerializedProperty m_panoramicRectTransform = null;
    private SerializedProperty m_tabletRectTransform = null;

    Editor m_panoramicRectTransformEditor = null;
    Editor m_tabletRectTransformEditor = null;

    private void OnEnable()
    {
        m_script = serializedObject.FindProperty("m_Script");
        m_panoramicRectTransform = serializedObject.FindProperty("m_panoramicRectTransform");
        m_tabletRectTransform = serializedObject.FindProperty("m_tabletRectTransform");

        m_panoramicRectTransformEditor = CreateEditor(m_panoramicRectTransform.objectReferenceValue);
        m_tabletRectTransformEditor = CreateEditor(m_tabletRectTransform.objectReferenceValue);
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        EditorGUILayout.PropertyField(m_script);
        EditorGUILayout.PropertyField(m_panoramicRectTransform);
        EditorGUILayout.PropertyField(m_tabletRectTransform);
        GUI.enabled = true;

        serializedObject.Update();

        string serializedEditorStr = EditorWindowsExtensions.IsTablet ? "Tablet" : "Panoramic";
        
        EditorGUILayout.LabelField(serializedEditorStr, EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(AsepectRatioAdapter.ResLog, MessageType.Warning);
        EditorGUILayout.HelpBox(EditorWindowsExtensions.ResLog, MessageType.Info);

        if (EditorWindowsExtensions.IsTablet)
            m_panoramicRectTransformEditor?.OnInspectorGUI();
        else
            m_tabletRectTransformEditor?.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
    }
}

public static class EditorWindowsExtensions
{
    public static bool IsTablet => GetMainGameViewSize().x / GetMainGameViewSize().y < 1.5f;
    public static string ResLog => $"Width: {GetMainGameViewSize().x} x Height: {GetMainGameViewSize().y} (AR: {GetMainGameViewSize().x / GetMainGameViewSize().y}) -> IsTablet: {EditorWindowsExtensions.IsTablet}";

    public static Vector2 GetMainGameViewSize()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        return (Vector2)GetSizeOfMainGameView.Invoke(null, null);
    }
}