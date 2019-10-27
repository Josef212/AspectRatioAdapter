using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(AsepectRatioAdapter))]
public class AsepectRatioAdapterEditor : Editor
{
    private SerializedProperty m_script = null;
    private SerializedProperty m_panoramicRectTransform = null;
    private SerializedProperty m_tabletRectTransform = null;

    private Editor m_panoramicRectTransformEditor = null;
    private Editor m_tabletRectTransformEditor = null;

    private RectTransform m_targetRectTransform = null;

    private SerializedProperty m_lastSerializedProperty = null;
    private bool dirty = false;

    private void OnEnable()
    {
        m_script = serializedObject.FindProperty("m_Script");
        m_panoramicRectTransform = serializedObject.FindProperty("m_panoramicRectTransform");
        m_tabletRectTransform = serializedObject.FindProperty("m_tabletRectTransform");

        m_panoramicRectTransformEditor = CreateEditor(m_panoramicRectTransform.objectReferenceValue);
        m_tabletRectTransformEditor = CreateEditor(m_tabletRectTransform.objectReferenceValue);

        m_targetRectTransform = (target as AsepectRatioAdapter).transform as RectTransform;
        m_lastSerializedProperty = null;
    }

    public override void OnInspectorGUI()
    {
        dirty = false;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(m_script);
        EditorGUILayout.PropertyField(m_panoramicRectTransform);
        EditorGUILayout.PropertyField(m_tabletRectTransform);
        GUI.enabled = true;

        serializedObject.Update();

        CheckSerializedTransformsHierarchy();

        string serializedEditorStr = ScreenHelper.IsTablet ? "Tablet" : "Panoramic";
        
        EditorGUILayout.LabelField(serializedEditorStr, EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(AsepectRatioAdapter.ResLog, MessageType.Warning);
        EditorGUILayout.HelpBox(ScreenHelper.ResLog, MessageType.Info);

        bool isTablet = ScreenHelper.IsTablet;
        Editor currentRectTransformEditor = isTablet ? m_tabletRectTransformEditor : m_panoramicRectTransformEditor;

        EditorGUI.BeginChangeCheck();
        currentRectTransformEditor?.OnInspectorGUI();

        SerializedProperty currentRectTransformSP = isTablet ? m_tabletRectTransform : m_panoramicRectTransform;

        if (EditorGUI.EndChangeCheck() || m_lastSerializedProperty != currentRectTransformSP)
        {
            m_lastSerializedProperty = currentRectTransformSP;

            // Copy current editting rect transform to the targe GO rect transform
            m_targetRectTransform.CopyFrom(currentRectTransformSP.objectReferenceValue as RectTransform);
        }

        serializedObject.ApplyModifiedProperties();

        if(dirty)
        {
            Repaint();
        }
    }

    private void CheckSerializedTransformsHierarchy()
    {
        RectTransform panoramicRect = m_panoramicRectTransform.objectReferenceValue as RectTransform;
        RectTransform tabletRect = m_tabletRectTransform.objectReferenceValue as RectTransform;

        if (panoramicRect.parent != m_targetRectTransform.parent)
        {
            panoramicRect.SetParent(m_targetRectTransform.parent, false);
            dirty = true;
        }

        if (tabletRect.parent != m_targetRectTransform.parent)
        {
            tabletRect.SetParent(m_targetRectTransform.parent, false);
            dirty = true;
        }
    }
}

public static class ScreenHelper
{
    private static System.Reflection.MethodInfo s_getSizeOfMainGameViewMethod = null;

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
            return $"Width: {res.x} x Height: {res.y} (AR: {res.x / res.y}) -> IsTablet: {IsTablet}";
        }
    }

    public static Vector2 GetMainGameViewSize()
    {
        if(s_getSizeOfMainGameViewMethod == null)
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            s_getSizeOfMainGameViewMethod = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        }

        return (Vector2)s_getSizeOfMainGameViewMethod.Invoke(null, null);
    }
}
