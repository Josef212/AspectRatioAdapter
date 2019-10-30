using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class AsepectRatioAdapter : MonoBehaviour
{
    public static bool IsTablet => Screen.width / (float)Screen.height < 1.5f;

    [SerializeField] private RectTransform m_panoramicRectTransform = null;
    [SerializeField] private RectTransform m_tabletRectTransform = null;

   
    private void Awake()
    {
        if(!Application.isPlaying)
            InitTransforms();

        RectTransform rect = transform as RectTransform;
        RectTransform reff = IsTablet ? m_tabletRectTransform : m_panoramicRectTransform;
        rect.CopyFrom(reff);
    }

    private void OnTransformParentChanged()
    {
        m_panoramicRectTransform?.SetParent(transform.parent);
        m_tabletRectTransform?.SetParent(transform.parent);
    }

#if UNITY_EDITOR
    private void Reset()
    {

    }
#endif

    private void InitTransforms()
    {
        CreateRectTransformIfNeeded(ref m_panoramicRectTransform, "PanoramicRectTransform");
        CreateRectTransformIfNeeded(ref m_tabletRectTransform, "TabletRectTransform");
    }

    private void CreateRectTransformIfNeeded(ref RectTransform rectTransform, string goName = "AspectRatioAdapterChild")
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
