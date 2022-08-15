using System;
using UnityEditor;
using UnityEngine;
using AP.Utilities.Extensions;

namespace AP.Utilities.Editor {
	[Serializable]
	public class TexturePreview {
		public enum Anchor {
			Left,
			Right,
			Center
		}
		
		private float width = 100;
		private float height = 100;
		
		public float Width => width * zoom;
		public float Height => height * zoom;
		
		public Vector2 Size {
			get => new Vector2(width, height);
			set {
				width = value.x;
				height = value.y;
			}
		}
		
		public Texture2D texture;
		private float zoom;
		private Color backgroundColor;
		public string label;
		public Color Color => backgroundColor;
		public Anchor anchor;
		public event Action<Vector2Int> Clicked;
		public event Action<Vector2> Scrolled;
		public event Action<Vector2Int> DragStart;
		public event Action DragEnd;
		
		public TexturePreview(Anchor anchor, string label = null) {
			texture = null;
			this.anchor = anchor;
			this.label = label;
			backgroundColor = Color.clear;
			zoom = 1;
		}
	
		public TexturePreview(ref Texture2D texture, float zoom, Color backgroundColor, Anchor anchor = Anchor.Left, string label = null) {
			this.texture = texture;
			this.backgroundColor = backgroundColor;
			this.zoom = zoom;
			this.anchor = anchor;
			this.label = label;
		}

		public TexturePreview(ref Texture2D texture, Anchor anchor = Anchor.Left, string label = null) {
			this.texture = texture;
			backgroundColor = Color.clear;
			this.anchor = anchor;
			this.label = label;
			zoom = 1;
		}
		public TexturePreview(Sprite sprite, Anchor anchor = Anchor.Left, string label = null) {
			texture = EditorUtilities.CopyTexture(sprite);
			backgroundColor = Color.clear;
			this.anchor = anchor;
			this.label = label;
			zoom = 1;
		}

		public void ChangeTexture(ref Texture2D newTexture) => texture = newTexture;
		
		public void ChangeTexture(Sprite sprite) => texture = EditorUtilities.CopyTexture(sprite);
		
		public void ChangeZoom(float newZoom) => zoom = newZoom;
		
		public void ChangeBackgroundColor(Color color) => backgroundColor = color;
		
		public void ChangeLabel(string newLabel) => label = newLabel;

		public void OnClicked(Vector2Int pos) => Clicked?.Invoke(pos);
		
		public void OnScrolled(Vector2 delta) => Scrolled?.Invoke(delta);
		
		public void OnDragStart(Vector2Int pos) => DragStart?.Invoke(pos);

		public void OnDragEnd() => DragEnd?.Invoke();

	}
	
	[CustomPropertyDrawer(typeof(TexturePreview))]
	public class TexturePreviewDrawer : PropertyDrawer {
		private const float SideBorder = 16f;
		private const float LabelHeight = 16f;
		private const float Space = 8f;

		private bool ScrollWheel => Event.current.type == EventType.ScrollWheel;
		private bool MouseDown => Event.current.type == EventType.MouseDown;
		private bool MouseUp => Event.current.type == EventType.MouseUp;
		private bool Drag => Event.current.type == EventType.MouseDrag;
		
		private bool dragging = false;
		private bool clicked = false;
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginChangeCheck();
			var preview = fieldInfo.GetValue(property.serializedObject.targetObject) as TexturePreview;
			
			bool hasLabel = !string.IsNullOrEmpty(preview.label);

			if (preview.texture == null) {
				EditorGUI.EndChangeCheck();
				return;
			}
				
			Rect rect = hasLabel 
				? EditorGUILayout.GetControlRect(false, preview.Height + LabelHeight * 3) 
				: EditorGUILayout.GetControlRect(false, preview.Height + LabelHeight);

			float additionalSpace = 0;
			
			float previewRectX = GetRectX(rect, preview);
			
			if (hasLabel) {
				string newLabel = preview.label;
				var style = new GUIStyle
				{
					fontSize = 20, 
					fontStyle = FontStyle.Bold, 
					alignment = TextAnchor.MiddleLeft
				};
				
				var labelRect = new Rect(previewRectX, rect.y, rect.width, LabelHeight);
				EditorGUI.LabelField(labelRect, newLabel, style);
				additionalSpace += (LabelHeight * 2);
			}
			
			var previewRect = new Rect(previewRectX, rect.position.y + additionalSpace , preview.Width, preview.Height);
			
			if (previewRect.Contains(Event.current.mousePosition)) 
				HandleInputs(preview, previewRect);
			
			EditorUtilities.DrawOuterBorders(previewRect, 2, Color.gray);
			Color guiColor = GUI.color;
			GUI.color = preview.Color;
			EditorGUI.DrawTextureTransparent(previewRect, preview.texture);
			GUI.color = guiColor;
			
			EditorGUI.EndChangeCheck();
		}
		
		private void HandleInputs(TexturePreview preview, Rect previewRect) {
			if (MouseUp) {
				clicked = false;
				if (dragging) {
					dragging = false;
					preview.OnDragEnd();
					Debug.Log("end drag");
				}
			}

			if (MouseDown || Drag) {
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
				if (Drag && !dragging) {
					Debug.Log("start drag");
					preview.OnDragStart(remappedPosition);
					dragging = true;
				}

				if (MouseDown && !clicked) {
					clicked = true;
					preview.OnClicked(remappedPosition);
				}
				Event.current.Use();
			}
			if (ScrollWheel) {
				preview.OnScrolled(Event.current.delta);
				Event.current.Use();
			}
		}
		
		private float GetRectX(Rect rect, TexturePreview preview) {
			switch (preview.anchor) {
				case TexturePreview.Anchor.Left: 
					return rect.x + SideBorder;
			
				case TexturePreview.Anchor.Right:
					return rect.xMax - SideBorder - preview.Width;
			
				case TexturePreview.Anchor.Center:
					return rect.center.x - (preview.Width/2);
			
				default:
					return rect.x + SideBorder;
			}
		}

		public override bool CanCacheInspectorGUI(SerializedProperty property) => false;
	}
}