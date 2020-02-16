using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class AspectRatioAdapter : UIBehaviour
{
    [SerializeField] private bool m_applyChangeOnPlayMode = false;

    [HideInInspector] [SerializeField] private RectTransform m_panoramicRectTransform = null;
    [HideInInspector] [SerializeField] private RectTransform m_tabletRectTransform = null;

    private RectTransform RectTransform => transform as RectTransform;
    
    protected override void Awake()
    {
        AspectRatioChecker.AspectRatioChanged += AspectRatioChanged;
        
#if UNITY_EDITOR
        if(!Application.isPlaying)
            InitTransforms();
#endif // UNITY_EDITOR

        ApplyNeededTransform(AspectRatioChecker.IsTablet);
    }

    protected override void OnDestroy()
    {
        AspectRatioChecker.AspectRatioChanged -= AspectRatioChanged;
    }

    protected void AspectRatioChanged(bool isTablet)
    {
        if (!Application.isPlaying || m_applyChangeOnPlayMode)
        {
            ApplyNeededTransform(isTablet);
        }

#if UNITY_EDITOR
        if (RectTransform.hasChanged && !Application.isPlaying && (UnityEditor.Selection.activeTransform == RectTransform || UnityEditor.Selection.transforms.Contains(RectTransform)))
        {
            (isTablet ? m_tabletRectTransform : m_panoramicRectTransform).CopyFrom(RectTransform);
        }
#endif
    }

    protected override void OnTransformParentChanged()
    {
        m_panoramicRectTransform?.SetParent(RectTransform.parent);
        m_tabletRectTransform?.SetParent(RectTransform.parent);
    }

#if UNITY_EDITOR
    protected override void Reset()
    {

    }
#endif // UNITY_EDITOR

    private void ApplyNeededTransform(bool isTable)
    {
        RectTransform reff = isTable ? m_tabletRectTransform : m_panoramicRectTransform;
        RectTransform.CopyFrom(reff);
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
            rectTransform.SetParent(RectTransform.parent, false);
        }
    }
}
