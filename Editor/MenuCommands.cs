using UnityEditor;
using UnityEngine;

namespace AP.Utilities.Editor
{
	public static class MenuCommands
	{
		[MenuItem("CONTEXT/RectTransform/Convert to Anchors", false, 0)]
		private static void ConvertToAnchors(MenuCommand command)
		{
			var rt = (RectTransform)command.context;
        
			if (rt == null) return;

			var parent = rt.parent as RectTransform;
			if (parent == null) return;
        
			Undo.RecordObject(rt, "Reset Anchors");

			var worldCorners = new Vector3[4];
			rt.GetWorldCorners(worldCorners);

			for (int i = 0; i < 4; i++)
				worldCorners[i] = parent.InverseTransformPoint(worldCorners[i]);

			Rect parentRect = parent.rect;

			Vector2 min = worldCorners[0];
			Vector2 max = worldCorners[2];

			var anchorMin = new Vector2(
				(min.x - parentRect.xMin) / parentRect.width,
				(min.y - parentRect.yMin) / parentRect.height
			);

			var anchorMax = new Vector2(
				(max.x - parentRect.xMin) / parentRect.width,
				(max.y - parentRect.yMin) / parentRect.height
			);

			rt.anchorMin = anchorMin;
			rt.anchorMax = anchorMax;

			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
		}
	}
}