using System;
using UnityEditor;
using UnityEngine;

namespace Utilities.Editor {
	[Serializable]
	public class TexturePreview {
		public float Width => 100 * zoom;
		public float Height => 100 * zoom;
		public Texture2D texture;
		public bool alignRight;
		private float zoom;
		private Color backgroundColor;
		public Color Color => backgroundColor;

		public TexturePreview(ref Texture2D texture, float zoom, Color backgroundColor, bool alignRight = false) {
			this.texture = texture;
			this.texture.alphaIsTransparency = true;
			this.texture.Apply();
			this.backgroundColor = backgroundColor;
			this.zoom = zoom;
			this.alignRight = alignRight;
		}

		public TexturePreview(ref Texture2D texture) {
			this.texture = texture;
			backgroundColor = Color.clear;
			zoom = 1;
		}

		public void ChangeTexture(ref Texture2D texture) {
			this.texture = texture;
		}

		public void ChangeZoom(float zoom) {
			this.zoom = zoom;
		}

		public void ChangeBackgroundColor(Color color) {
			backgroundColor = color;
		}
	}
	
	[CustomPropertyDrawer(typeof(TexturePreview))]
	public class TexturePreviewDrawer : PropertyDrawer {
		private const float SideBorder = 16f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginChangeCheck();
			var preview = fieldInfo.GetValue(property.serializedObject.targetObject) as TexturePreview;

			if (preview.texture != null) {
				float rectX = preview.alignRight ? position.xMax - SideBorder - preview.Width : position.x + SideBorder;
				var previewRect = new Rect(rectX, position.position.y, preview.Width, preview.Height);
				EditorUtility.DrawBorders(previewRect, 2, Color.gray);
				Color guiColor = GUI.color;
				GUI.color = preview.Color;
				EditorGUI.DrawTextureTransparent(previewRect, preview.texture);
				GUI.color = guiColor;
			}
			EditorGUI.EndChangeCheck();
		}
		
		public override bool CanCacheInspectorGUI(SerializedProperty property)
		{
			return false;
		}
	}
}