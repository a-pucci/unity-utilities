using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utilities.Editor {
	public static class EditorUtilities {
		
		/// <summary>
		/// Retrieve all ScriptableObjects of a Type
		/// </summary>
		/// <typeparam name="T">ScriptableObject</typeparam>
		public static T[] GetAllInstances<T>() where T : ScriptableObject {
			string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
			var a = new T[guids.Length];
			for (int i = 0; i < guids.Length; i++) {
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
			}
			return a;
		}
		
		/// <summary>
		/// Get a single Sprite from the Texture path
		/// </summary>
		/// <param name="path">Texture path</param>
		/// <param name="name">Sprite Name</param>
		public static Sprite GetSpriteFromSheet(string path, string name) {
			Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
			return Array.Find(sprites, s => s.name == name);
		}

		/// <summary>
		/// Create ScriptableObject asset in a specified path
		/// </summary>
		/// <param name="path">Asset path</param>
		/// <param name="name">Asset name</param>
		/// <typeparam name="T">Asset Type</typeparam>
		public static T CreateAsset<T>(string path, string name) where T : ScriptableObject {
			var asset = ScriptableObject.CreateInstance<T>();
			AssetDatabase.CreateAsset(asset, $"{path}/{name}.asset");
			EditorUtility.SetDirty(asset);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			return asset;
		}
		
		/// <summary>
		/// Get the folder path containing the Asset
		/// </summary>
		/// <param name="asset">Asset</param>
		/// <typeparam name="T">ScriptableObject</typeparam>
		/// <returns></returns>
		public static string GetFolderPath<T>(T asset) where T : ScriptableObject {
			string assetPath = AssetDatabase.GetAssetPath(asset);
			return Path.GetDirectoryName(assetPath);
		}
		
		/// <summary>
		/// Create or load an Asset given its path and the asset reference
		/// </summary>
		/// <param name="path">Asset path</param>
		/// <param name="asset">Asset reference</param>
		/// <typeparam name="T">ScriptableObject</typeparam>
		public static T CreateOrLoad<T>(string path, T asset) where T : ScriptableObject {
			var obj = AssetDatabase.LoadAssetAtPath<T>(path);
			
			if (obj == null) {
				AssetDatabase.CreateAsset(asset, path);
				AssetDatabase.SaveAssets();
				obj = AssetDatabase.LoadAssetAtPath<T>(path);
			}
			
			EditorUtility.SetDirty(obj);
			return obj;
		}

		/// <summary>
		/// Create or load an Asset given its path
		/// </summary>
		/// <param name="path">Asset path</param>
		/// <typeparam name="T">ScriptableObject</typeparam>
		public static T CreateOrLoad<T>(string path) where T : ScriptableObject {
			var obj = AssetDatabase.LoadAssetAtPath<T>(path);
			
			if (obj == null) {
				obj = ScriptableObject.CreateInstance<T>();
				AssetDatabase.CreateAsset(obj, path);
				AssetDatabase.SaveAssets();
				obj = AssetDatabase.LoadAssetAtPath<T>(path);
			}
			
			EditorUtility.SetDirty(obj);
			return obj;
		}

		/// <summary>
		/// Delete Asset
		/// </summary>
		/// <typeparam name="T">ScriptableObject</typeparam>
		public static bool DeleteAsset<T>(T asset) where T : ScriptableObject {
			string path = AssetDatabase.GetAssetPath(asset);
			
			if (string.IsNullOrEmpty(path))
				return false;
			
			if (AssetDatabase.DeleteAsset(path)) {
				AssetDatabase.Refresh();
				return true;
			}
			
			return false;
		}

		/// <summary>
		/// Rename Asset
		/// </summary>
		/// <param name="asset">Asset to rename</param>
		/// <param name="newName">Asset new name</param>
		/// <typeparam name="T"></typeparam>
		public static void Rename<T>(T asset, string newName) where T : ScriptableObject {
			string path = AssetDatabase.GetAssetPath(asset);
			
			if (!string.IsNullOrEmpty(path))
				AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(asset), newName);
			
			EditorUtility.SetDirty(asset);
		}
		
		///  Copy sprite texture within sprite rect
		public static Texture2D CopyTexture(Sprite sprite) {
			var newTexture = new Texture2D((int)sprite.textureRect.size.x, (int)sprite.textureRect.size.y, TextureFormat.RGBA32, false)
				{ filterMode = FilterMode.Point, alphaIsTransparency = true };
			Color[] pixels = sprite.texture.GetPixels(
				(int)sprite.textureRect.x,
				(int)sprite.textureRect.y,
				(int)sprite.textureRect.width,
				(int)sprite.textureRect.height);

			newTexture.SetPixels(pixels);
			newTexture.Apply();

			return newTexture;
		}

		/// <summary>
		/// Copy part of texture
		/// </summary>
		/// <param name="texture">Texture to copy</param>
		/// <param name="rect">Texture Rect to copy</param>
		/// <returns></returns>
		public static Texture2D CopyTexture(Texture2D texture, Rect rect) {
			var newTexture = new Texture2D((int)rect.size.x, (int)rect.size.y, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, alphaIsTransparency = true };
			Color[] pixels = texture.GetPixels(
				(int)rect.x,
				(int)rect.y,
				(int)rect.width,
				(int)rect.height);

			newTexture.SetPixels(pixels);
			newTexture.Apply();

			return newTexture;
		}

		/// Copy whole texture
		public static Texture2D CopyTexture(Texture2D texture) {
			var newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, alphaIsTransparency = true };
			Color[] pixels = texture.GetPixels(
				0,
				0,
				texture.width,
				texture.height);

			newTexture.SetPixels(pixels);
			newTexture.Apply();

			return newTexture;
		}

		public static void DrawVerticalLine(float x, float startY, float endY, int lineWidth, Color color) {
			var lineRect = new Rect(x, startY, lineWidth, endY - startY);
			EditorGUI.DrawRect(lineRect, color);
		}

		public static void DrawHorizontalLine(float y, float startX, float endX, int lineHeight, Color color) {
			var lineRect = new Rect(startX, y, endX - startX, lineHeight);
			EditorGUI.DrawRect(lineRect, color);
		}

		/// Draw border outside the given Rect
		public static Rect DrawOuterBorders(Rect rect, int borderWidth, Color color) {
			var newRect = new Rect(rect.x - borderWidth, rect.y - borderWidth, rect.width + borderWidth * 2, rect.height + borderWidth * 2);

			if (Event.current?.type != EventType.Repaint)
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

		/// Draw border inside the given Rect
		public static Rect DrawInnerBorders(Rect rect, int borderWidth, Color color) {
			var newRect = new Rect(rect.x + borderWidth, rect.y + borderWidth, rect.width - borderWidth * 2, rect.height - borderWidth * 2);
			if (Event.current?.type != EventType.Repaint)
				return newRect;

			if (borderWidth > 0) {
				Rect rect1 = rect;
				rect1.width = borderWidth;
				EditorGUI.DrawRect(rect1, color);

				rect1 = rect;
				rect1.height = borderWidth;
				EditorGUI.DrawRect(rect1, color);

				rect1 = rect;
				rect1.x += rect.width - borderWidth;
				rect1.width = borderWidth;
				EditorGUI.DrawRect(rect1, color);

				rect1 = rect;
				rect1.y += rect.height - borderWidth;
				rect1.height = borderWidth;
				EditorGUI.DrawRect(rect1, color);
			}
			return newRect;
		}

		/// Draw a Title label
		public static void DrawTitle(string label, int size, int space, bool drawLine = true) {
			GUILayout.Space(space);
			
			var style = new GUIStyle { fontSize = size, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
			Rect r = EditorGUILayout.GetControlRect(false, size + space * 2);
			var labelRect = new Rect(r.x, r.y + space, r.width, size);
			
			EditorGUI.LabelField(labelRect, label, style);
			if (drawLine) {
				float width = r.width;
				var lineRect = new Rect(r.center.x - width / 2, labelRect.y + size + (space / 2), width, 0);
				DrawOuterBorders(lineRect, 1, Color.black);
			}
			
			GUILayout.Space(space);
		}

		/// Draw a color box
		public static void DrawBox(Vector2 size, Color color, float space) {
			Rect r = EditorGUILayout.GetControlRect(false, size.y);
			var rect = new Rect(r.x + space, r.y + 1, size.x, size.y);
			EditorGUI.DrawRect(rect, color);
		}
	}
}