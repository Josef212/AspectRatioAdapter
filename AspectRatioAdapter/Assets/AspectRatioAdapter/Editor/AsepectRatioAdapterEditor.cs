﻿using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(AsepectRatioAdapter))]
public class AsepectRatioAdapterEditor : Editor
{
    private SerializedProperty m_script = null;
    private SerializedProperty m_applyChangeOnPlayMode = null;
    private SerializedProperty m_panoramicRectTransform = null;
    private SerializedProperty m_tabletRectTransform = null;

    private Editor m_panoramicRectTransformEditor = null;
    private Editor m_tabletRectTransformEditor = null;

    private RectTransform m_targetRectTransform = null;
    private DrivenRectTransformTracker m_tracker;
    private bool m_otherRectFold = false;

    private SerializedProperty m_lastSerializedProperty = null;

    private GUIStyle m_boldFoldoutStyle = null;

    private void OnEnable()
    {
        m_script = serializedObject.FindProperty("m_Script");
        m_applyChangeOnPlayMode = serializedObject.FindProperty("m_applyChangeOnPlayMode");
        m_panoramicRectTransform = serializedObject.FindProperty("m_panoramicRectTransform");
        m_tabletRectTransform = serializedObject.FindProperty("m_tabletRectTransform");

        m_panoramicRectTransformEditor = CreateEditor(m_panoramicRectTransform.objectReferenceValue);
        m_tabletRectTransformEditor = CreateEditor(m_tabletRectTransform.objectReferenceValue);

        m_targetRectTransform = (target as AsepectRatioAdapter).transform as RectTransform;
        m_lastSerializedProperty = null;

        m_boldFoldoutStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold
        };

        if (!Application.isPlaying)
            m_tracker.Add(target, m_targetRectTransform, DrivenTransformProperties.All);
    }

    private void OnDisable()
    {
        m_tracker.Clear();
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
        EditorGUILayout.HelpBox(ScreenHelper.ResLog, MessageType.Info);
        EditorGUILayout.LabelField(serializedEditorStr, EditorStyles.boldLabel);

        Editor currentRectTransformEditor = isTablet ? m_tabletRectTransformEditor : m_panoramicRectTransformEditor;

        EditorGUI.BeginChangeCheck();
        currentRectTransformEditor?.OnInspectorGUI();

        SerializedProperty currentRectTransformSP = isTablet ? m_tabletRectTransform : m_panoramicRectTransform;

        if (EditorGUI.EndChangeCheck())// || m_lastSerializedProperty != currentRectTransformSP)
        {
            m_lastSerializedProperty = currentRectTransformSP;
            
            if (!Application.isPlaying)
            {
                m_tracker.Clear();
                DrivenRectTransformTracker.StartRecordingUndo();
            }

            m_targetRectTransform.CopyFrom(currentRectTransformSP.objectReferenceValue as RectTransform);
            if (!Application.isPlaying)
            {
                m_tracker.Add(target, m_targetRectTransform, DrivenTransformProperties.All);
                DrivenRectTransformTracker.StopRecordingUndo();
            }
        }
        
        m_otherRectFold = EditorGUILayout.Foldout(m_otherRectFold,
                    isTablet ? "Panoramic RectTransform" : "Tablet RectTransform",
                    m_boldFoldoutStyle);
        if (m_otherRectFold)
        {
            GUI.enabled = false;

            Editor otherEditor = (currentRectTransformEditor == m_panoramicRectTransformEditor
                ? m_tabletRectTransformEditor : m_panoramicRectTransformEditor);
            otherEditor?.OnInspectorGUI();

            GUI.enabled = true;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
