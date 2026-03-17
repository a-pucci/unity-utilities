using UnityEngine;

namespace AP.Utilities
{
	public static class ScreenHelper
	{
		public static Vector2 ScreenSize =>
#if UNITY_EDITOR
			UnityEditor.Handles.GetMainGameViewSize();
#else
			new Vector2(Screen.width, Screen.height);
#endif

		public static bool IsPortrait => ScreenSize.y > ScreenSize.x;

		public static float AspectRatio
		{
			get
			{
				Vector2 size = ScreenSize;
				return Mathf.Max(size.x, size.y) / Mathf.Min(size.x, size.y);
			}
		}
	}
}