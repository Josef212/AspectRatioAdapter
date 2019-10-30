using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class AsepectRatioAdapter : UIBehaviour
{
    public static bool IsTablet => Screen.width / (float)Screen.height < 1.5f;
    private static bool WasTablet = IsTablet;

    [SerializeField] private bool m_applyChangeOnPlayMode = false;

    [HideInInspector] [SerializeField] private RectTransform m_panoramicRectTransform = null;
    [HideInInspector] [SerializeField] private RectTransform m_tabletRectTransform = null;


    protected override void Awake()
    {
#if UNITY_EDITOR
        if(!Application.isPlaying)
            InitTransforms();
#endif // UNITY_EDITOR

        ApplyNeededTransform();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        if (!Application.isPlaying || m_applyChangeOnPlayMode)
        {
            if (WasTablet != IsTablet)
            {
                WasTablet = IsTablet;
                ApplyNeededTransform();
            }
        }
    }
        
    protected override void OnTransformParentChanged()
    {
        m_panoramicRectTransform?.SetParent(transform.parent);
        m_tabletRectTransform?.SetParent(transform.parent);
    }

#if UNITY_EDITOR
    protected override void Reset()
    {

    }
#endif // UNITY_EDITOR

    private void ApplyNeededTransform()
    {
        RectTransform rect = transform as RectTransform;
        RectTransform reff = IsTablet ? m_tabletRectTransform : m_panoramicRectTransform;
        rect.CopyFrom(reff);
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
            rectTransform.SetParent(transform.parent, false);
        }
    }
}
