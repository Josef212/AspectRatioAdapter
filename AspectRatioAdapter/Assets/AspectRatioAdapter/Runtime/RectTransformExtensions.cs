using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtensions
{
    public static void CopyFrom(this RectTransform self, RectTransform other)
    {
        self.pivot = other.pivot;
        self.sizeDelta = other.sizeDelta;
        self.anchoredPosition = other.anchoredPosition;
        self.anchorMax = other.anchorMax;
        self.anchorMin = other.anchorMin;
        self.offsetMax = other.offsetMax;
        self.offsetMin = other.offsetMin;
        self.localPosition = other.localPosition;
        self.localRotation = other.localRotation;
        self.localScale = other.localScale;
    }
}
