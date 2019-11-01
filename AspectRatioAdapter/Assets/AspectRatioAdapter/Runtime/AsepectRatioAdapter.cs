using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class AsepectRatioAdapter : UIBehaviour
{
    public static bool IsTablet => ScreenHelper.IsTablet;
    private static bool WasTablet = IsTablet;

    [SerializeField] private bool m_applyChangeOnPlayMode = false;

    [HideInInspector] [SerializeField] private RectTransform m_panoramicRectTransform = null;
    [HideInInspector] [SerializeField] private RectTransform m_tabletRectTransform = null;

    private RectTransform m_rectTransform = null;


    protected override void Awake()
    {
        m_rectTransform = transform as RectTransform;
#if UNITY_EDITOR
        if(!Application.isPlaying)
            InitTransforms();
#endif // UNITY_EDITOR

        ApplyNeededTransform();
    }

    protected void Update()
    {
        if (!Application.isPlaying || m_applyChangeOnPlayMode)
        {
            if (WasTablet != IsTablet)
            {
                WasTablet = IsTablet;
                ApplyNeededTransform();
            }
        }

#if UNITY_EDITOR
        if (m_rectTransform.hasChanged && !Application.isPlaying && (UnityEditor.Selection.activeTransform == m_rectTransform || UnityEditor.Selection.transforms.Contains(m_rectTransform)))
        {
            (IsTablet ? m_tabletRectTransform : m_panoramicRectTransform).CopyFrom(m_rectTransform);
        }
#endif
    }

    protected override void OnTransformParentChanged()
    {
        m_panoramicRectTransform?.SetParent(m_rectTransform.parent);
        m_tabletRectTransform?.SetParent(m_rectTransform.parent);
    }

#if UNITY_EDITOR
    protected override void Reset()
    {

    }
#endif // UNITY_EDITOR

    private void ApplyNeededTransform()
    {
        RectTransform reff = ScreenHelper.IsTablet ? m_tabletRectTransform : m_panoramicRectTransform;
        m_rectTransform.CopyFrom(reff);
    }

    private void InitTransforms()
    {
        CreateRectTransformIfNeeded(ref m_panoramicRectTransform, "PanoramicRectTransform");
        CreateRectTransformIfNeeded(ref m_tabletRectTransform, "TabletRectTransform");
    }

    private void CreateRectTransformIfNeeded(ref RectTransform rectTransform, string goName)
    {
        if(rectTransform == null)
        {
            GameObject child = new GameObject(goName);
            child.hideFlags = HideFlags.HideInHierarchy;
            rectTransform = child.AddComponent<RectTransform>();
            rectTransform.SetParent(m_rectTransform.parent, false);
        }
    }
}
