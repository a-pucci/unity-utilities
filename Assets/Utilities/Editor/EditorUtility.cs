using UnityEditor;
using UnityEngine;

namespace Utilities.Editor {
	public static class EditorUtility {
		public static Texture2D CopyTexture(Sprite sprite) {
			var newTexture = new Texture2D((int)sprite.textureRect.size.x, (int)sprite.textureRect.size.y, TextureFormat.RGBA32, false) {filterMode = FilterMode.Point, alphaIsTransparency = true};
			Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
				(int)sprite.textureRect.y,
				(int)sprite.textureRect.width,
				(int)sprite.textureRect.height);

			newTexture.SetPixels(pixels);
			newTexture.Apply();
			return newTexture;
		}

		public static Texture2D CopyTexture(Texture2D texture, Rect rect) {
			var newTexture = new Texture2D((int)rect.size.x, (int)rect.size.y, TextureFormat.RGBA32, false) {filterMode = FilterMode.Point, alphaIsTransparency = true};
			Color[] pixels = texture.GetPixels((int)rect.x,
				(int)rect.y,
				(int)rect.width,
				(int)rect.height);

			newTexture.SetPixels(pixels);
			newTexture.Apply();
			return newTexture;
		}
	
		public static Rect DrawBorders(Rect rect, int borderWidth, Color color) {		
			var newRect = new Rect(rect.x - borderWidth, rect.y - borderWidth , rect.width + borderWidth*2, rect.height + borderWidth*2);
		
			if (Event.current.type != EventType.Repaint)
				return newRect;

			if (borderWidth > 0) {
				// Left
				Rect rect1 = newRect;
				rect1.width = borderWidth;
				EditorGUI.DrawRect(rect1, color);
			
				// Top
				rect1 = newRect;
				rect1.height = borderWidth;
				EditorGUI.DrawRect(rect1, color);

				// Right
				rect1 = newRect;
				rect1.x += newRect.width - borderWidth;
				rect1.width = borderWidth;
				EditorGUI.DrawRect(rect1, color);
			
				// Bottom
				rect1 = newRect;
				rect1.y += newRect.height - borderWidth;
				rect1.height = borderWidth;
				EditorGUI.DrawRect(rect1, color);
			}
			return newRect;
		}
	}
}