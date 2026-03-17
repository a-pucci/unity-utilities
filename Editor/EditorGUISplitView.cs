using UnityEditor;
using UnityEngine;

namespace AP.Utilities.Editor
{
	public class EditorGUISplitView
	{
		public enum Direction
		{
			Horizontal,
			Vertical
		}

		private readonly Direction splitDirection;
		private static float splitNormalizedPosition;
		private bool resize;
		public Vector2 scrollPosition;
		private Rect availableRect;

		public EditorGUISplitView(Direction splitDirection, float percentage = 0.5f)
		{
			splitNormalizedPosition = percentage;
			this.splitDirection = splitDirection;
		}

		public void BeginSplitView()
		{
			Rect tempRect;

			if (splitDirection == Direction.Horizontal)
				tempRect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			else
				tempRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));

			if (tempRect.width > 0.0f)
			{
				availableRect = tempRect;
			}
			if (splitDirection == Direction.Horizontal)
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(availableRect.width * splitNormalizedPosition));
			else
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(availableRect.height * splitNormalizedPosition));
		}

		public void Split()
		{
			GUILayout.EndScrollView();
			GUILayout.Space(10);
			ResizeSplitFirstView();
		}

		public void EndSplitView()
		{
			if (splitDirection == Direction.Horizontal)
				EditorGUILayout.EndHorizontal();
			else
				EditorGUILayout.EndVertical();
		}

		private void ResizeSplitFirstView()
		{
			Rect resizeHandleRect;

			if (splitDirection == Direction.Horizontal)
				resizeHandleRect = new Rect(availableRect.width * splitNormalizedPosition, availableRect.y, 2f, availableRect.height);
			else
				resizeHandleRect = new Rect(availableRect.x - 5, (availableRect.height * splitNormalizedPosition) + 10, availableRect.width + 10, 3f);

			GUI.color = Color.black;
			GUI.contentColor = Color.black;
			GUI.DrawTexture(resizeHandleRect, EditorGUIUtility.whiteTexture);
			GUI.color = Color.white;
			GUI.contentColor = Color.white;


			if (splitDirection == Direction.Horizontal)
				EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.SplitResizeLeftRight);
			else
				EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.SplitResizeUpDown);

			if (Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(Event.current.mousePosition))
			{
				resize = true;
			}
			if (resize)
			{
				if (splitDirection == Direction.Horizontal)
					splitNormalizedPosition = Event.current.mousePosition.x / availableRect.width;
				else
					splitNormalizedPosition = Event.current.mousePosition.y / availableRect.height;
			}
			if (Event.current.type == EventType.MouseUp)
				resize = false;
		}

		#region Custom

		public void BeginSplitView(float percentage)
		{
			Rect tempRect;

			if (splitDirection == Direction.Horizontal)
				tempRect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			else
				tempRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));

			if (tempRect.width > 0.0f)
			{
				availableRect = tempRect;
			}
			if (splitDirection == Direction.Horizontal)
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(availableRect.width * percentage));
			else
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(availableRect.height * percentage));
		}

		public float Split(float percentage)
		{
			GUILayout.EndScrollView();
			GUILayout.Space(10);
			return ResizeSplitFirstView(percentage);
		}

		private float ResizeSplitFirstView(float percentage)
		{
			Rect resizeHandleRect;

			if (splitDirection == Direction.Horizontal)
				resizeHandleRect = new Rect(availableRect.width * percentage, availableRect.y, 2f, availableRect.height);
			else
				resizeHandleRect = new Rect(availableRect.x - 5, (availableRect.height * percentage) + 10, availableRect.width + 10, 3f);

			GUI.color = Color.black;
			GUI.contentColor = Color.black;
			GUI.DrawTexture(resizeHandleRect, EditorGUIUtility.whiteTexture);
			GUI.color = Color.white;
			GUI.contentColor = Color.white;


			if (splitDirection == Direction.Horizontal)
				EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeHorizontal);
			else
				EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeVertical);

			if (Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(Event.current.mousePosition))
			{
				resize = true;
			}
			if (resize)
			{
				if (splitDirection == Direction.Horizontal)
					percentage = Event.current.mousePosition.x / availableRect.width;
				else
					percentage = Event.current.mousePosition.y / availableRect.height;
			}
			if (Event.current.type == EventType.MouseUp)
				resize = false;
			return percentage;
		}

		#endregion
	}
}