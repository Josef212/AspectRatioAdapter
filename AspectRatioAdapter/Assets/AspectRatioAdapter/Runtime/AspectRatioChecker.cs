using System;
using UnityEngine;

public delegate void AspectRatioChanged(bool isTablet);

[ExecuteAlways]
public class AspectRatioChecker : MonoBehaviour
{
	public static AspectRatioChecker Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = FindObjectOfType<AspectRatioChecker>();
				if(s_instance == null)
				{
					var go = new GameObject("AspectRatioChecker");
					go.hideFlags = HideFlags.HideAndDontSave;
					s_instance = go.AddComponent<AspectRatioChecker>();
				}
			}

			var instances = FindObjectsOfType<AspectRatioChecker>();
			if(instances.Length > 1)
			{
				for(int i = 1; i < instances.Length; ++i)
				{
					Destroy(instances[i].gameObject);
				}
			}
			
			return s_instance;
			
		}
	}

	public static bool IsTablet => ScreenHelper.IsTablet;
	public static bool WasTablet => Instance.m_wasTablet;
	public static event AspectRatioChanged AspectRatioChanged
	{
		add => Instance.m_aspectRatioChanged += value;
		remove => Instance.m_aspectRatioChanged -= value;
	}

	private void Update()
	{
		bool isTablet = ScreenHelper.IsTablet;
		if(isTablet != m_wasTablet)
		{
			m_aspectRatioChanged?.Invoke(isTablet);
			m_wasTablet = isTablet;
		}
	}

	private static AspectRatioChecker s_instance = null;
	private bool m_wasTablet = false;
	private event AspectRatioChanged m_aspectRatioChanged;
}
