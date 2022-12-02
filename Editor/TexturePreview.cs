using System;
using AP.Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace AP.Utilities.Editor
{
	[Serializable]
	public class TexturePreview
	{
		public enum Anchor
		{
			Left,
			Right,
			Center
		}

		public Texture2D texture;
		public string label;
		public Anchor anchor;
		private float height = 100;

		private float width = 100;
		private float zoom;

		public TexturePreview(Anchor anchor, string label = null)
		{
			texture = null;
			this.anchor = anchor;
			this.label = label;
			Color = Color.clear;
			zoom = 1;
		}

		public TexturePreview(ref Texture2D texture, float zoom, Color backgroundColor, Anchor anchor = Anchor.Left, string label = null)
		{
			this.texture = texture;
			Color = backgroundColor;
			this.zoom = zoom;
			this.anchor = anchor;
			this.label = label;
		}

		public TexturePreview(ref Texture2D texture, Anchor anchor = Anchor.Left, string label = null)
		{
			this.texture = texture;
			Color = Color.clear;
			this.anchor = anchor;
			this.label = label;
			zoom = 1;
		}

		public TexturePreview(Sprite sprite, Anchor anchor = Anchor.Left, string label = null)
		{
			texture = EditorUtilities.CopyTexture(sprite);
			Color = Color.clear;
			this.anchor = anchor;
			this.label = label;
			zoom = 1;
		}

		public float Width => width * zoom;
		public float Height => height * zoom;

		public Vector2 Size
		{
			get => new Vector2(width, height);
			set
			{
				width = value.x;
				height = value.y;
			}
		}
		public Color Color { get; private set; }
		public event Action<Vector2Int> Clicked;
		public event Action<Vector2> Scrolled;
		public event Action<Vector2Int> DragStart;
		public event Action DragEnd;

		public void ChangeTexture(ref Texture2D newTexture) => texture = newTexture;

		public void ChangeTexture(Sprite sprite) => texture = EditorUtilities.CopyTexture(sprite);

		public void ChangeZoom(float newZoom) => zoom = newZoom;

		public void ChangeBackgroundColor(Color color) => Color = color;

		public void ChangeLabel(string newLabel) => label = newLabel;

		public void OnClicked(Vector2Int pos) => Clicked?.Invoke(pos);

		public void OnScrolled(Vector2 delta) => Scrolled?.Invoke(delta);

		public void OnDragStart(Vector2Int pos) => DragStart?.Invoke(pos);

		public void OnDragEnd() => DragEnd?.Invoke();
	}

	[CustomPropertyDrawer(typeof(TexturePreview))]
	public class TexturePreviewDrawer : PropertyDrawer
	{
		private const float SideBorder = 16f;
		private const float LabelHeight = 16f;
		private const float Space = 8f;
		private bool clicked;

		private bool dragging;

		private static bool ScrollWheel => Event.current.type == EventType.ScrollWheel;
		private static bool MouseDown => Event.current.type == EventType.MouseDown;
		private static bool MouseUp => Event.current.type == EventType.MouseUp;
		private static bool Drag => Event.current.type == EventType.MouseDrag;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginChangeCheck();
			var preview = fieldInfo.GetValue(property.serializedObject.targetObject) as TexturePreview;

			bool hasLabel = !string.IsNullOrEmpty(preview.label);

			if (preview.texture == null)
			{
				EditorGUI.EndChangeCheck();

				return;
			}

			Rect rect = hasLabel
				? EditorGUILayout.GetControlRect(false, preview.Height + LabelHeight * 3)
				: EditorGUILayout.GetControlRect(false, preview.Height + LabelHeight);

			float additionalSpace = 0;

			float previewRectX = GetRectX(rect, preview);

			if (hasLabel)
			{
				string newLabel = preview.label;
				var style = new GUIStyle
				{
					fontSize = 20,
					fontStyle = FontStyle.Bold,
					alignment = TextAnchor.MiddleLeft
				};

				var labelRect = new Rect(previewRectX, rect.y, rect.width, LabelHeight);
				EditorGUI.LabelField(labelRect, newLabel, style);
				additionalSpace += LabelHeight * 2;
			}

			var previewRect = new Rect(previewRectX, rect.position.y + additionalSpace, preview.Width, preview.Height);

			if (previewRect.Contains(Event.current.mousePosition))
				HandleInputs(preview, previewRect);

			EditorUtilities.DrawOuterBorders(previewRect, 2, Color.gray);
			Color guiColor = GUI.color;
			GUI.color = preview.Color;
			EditorGUI.DrawTextureTransparent(previewRect, preview.texture);
			GUI.color = guiColor;

			EditorGUI.EndChangeCheck();
		}

		private void HandleInputs(TexturePreview preview, Rect previewRect)
		{
			if (MouseUp)
			{
				clicked = false;

				if (dragging)
				{
					dragging = false;
					preview.OnDragEnd();
				}
			}

			if (MouseDown || Drag)
			{
				var mouseToRectPosition = new Vector2
				{
					x = Event.current.mousePosition.x - previewRect.x,
					y = Event.current.mousePosition.y - previewRect.y
				};

				var remappedPosition = new Vector2Int
				{
					x = Mathf.FloorToInt(mouseToRectPosition.x.Remap(0, preview.Width, 0, preview.texture.width)),
					y = Mathf.FloorToInt(mouseToRectPosition.y.Remap(0, preview.Height, preview.texture.height, 0))
				};

				if (Drag && !dragging)
				{
					preview.OnDragStart(remappedPosition);
					dragging = true;
				}

				if (MouseDown && !clicked)
				{
					clicked = true;
					preview.OnClicked(remappedPosition);
				}

				Event.current.Use();
			}

			if (ScrollWheel)
			{
				preview.OnScrolled(Event.current.delta);
				Event.current.Use();
			}
		}

		private float GetRectX(Rect rect, TexturePreview preview)
		{
			return preview.anchor switch
			{
				TexturePreview.Anchor.Left => rect.x + SideBorder,
				TexturePreview.Anchor.Right => rect.xMax - SideBorder - preview.Width,
				TexturePreview.Anchor.Center => rect.center.x - preview.Width / 2,
				_ => rect.x + SideBorder
			};
		}

		public override bool CanCacheInspectorGUI(SerializedProperty property) => false;
	}
}