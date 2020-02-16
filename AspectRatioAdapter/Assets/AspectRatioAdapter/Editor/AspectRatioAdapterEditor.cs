using System;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(AspectRatioAdapter))]
public class AspectRatioAdapterEditor : Editor
{
    private SerializedProperty m_script = null;
    private SerializedProperty m_applyChangeOnPlayMode = null;
    private SerializedProperty m_panoramicRectTransform = null;
    private SerializedProperty m_tabletRectTransform = null;

    private bool m_otherRectFold = false;

    private Lazy<GUIStyle> m_boldFoldoutStyle = new Lazy<GUIStyle>(() => new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

    private void OnEnable()
    {
        m_script = serializedObject.FindProperty("m_Script");
        m_applyChangeOnPlayMode = serializedObject.FindProperty("m_applyChangeOnPlayMode");
        m_panoramicRectTransform = serializedObject.FindProperty("m_panoramicRectTransform");
        m_tabletRectTransform = serializedObject.FindProperty("m_tabletRectTransform");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(m_script);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(m_applyChangeOnPlayMode, 
            new GUIContent("Apply On Play", "Rect transform change will be applied if an aspect ration change is detected during play mode too."), 
            true);

        EditorGUILayout.Space();
                
        bool isTablet = ScreenHelper.IsTablet;

        string serializedEditorStr = isTablet ? "Tablet" : "Panoramic";
        EditorGUILayout.LabelField($"Current editing: {serializedEditorStr}", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(ScreenHelper.ResLog, MessageType.Info);

        m_otherRectFold = EditorGUILayout.Foldout(m_otherRectFold,
                    isTablet ? "Panoramic RectTransform" : "Tablet RectTransform",
                    m_boldFoldoutStyle.Value);
        if (m_otherRectFold)
        {
            GUI.enabled = false;
            Editor otherEditor = CreateEditor((isTablet ? m_panoramicRectTransform : m_tabletRectTransform).objectReferenceValue);
            otherEditor?.OnInspectorGUI();
            GUI.enabled = true;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
