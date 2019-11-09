using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugInfo : MonoBehaviour
{
    private void Awake()
    {
        int w = Screen.width, h = Screen.height;
        m_style.alignment = TextAnchor.UpperRight;
        m_style.fontSize = h * 3 / 100;
        m_style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        Rect m_rect = new Rect(0, h - (h * 5 / 100), w, h * 2 / 100);

        GUI.Label(m_rect, $"Width: {Screen.width} x Height: {Screen.height} (AR: {Screen.width / (float)Screen.height}) -> IsTablet: {ScreenHelper.IsTablet}", m_style);
    }

    private GUIStyle m_style = new GUIStyle();
}
