using System;
using System.Reflection;
using UnityEditor;

namespace AP.Utilities.UI.Editor
{
	public static class GameViewUtils
	{
		private static readonly object GameViewSizesInstance;
		private static MethodInfo getGroup;

		static GameViewUtils()
		{
			Type sizesType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameViewSizes");
			Type singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
			PropertyInfo instanceProp = singleType.GetProperty("instance");
			getGroup = sizesType.GetMethod("GetGroup");
			GameViewSizesInstance = instanceProp.GetValue(null, null);
		}

		public static void SetSize(int index)
		{
			if (index == -1)
				return;

			Type gvWndType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");
			PropertyInfo selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			EditorWindow gvWnd = EditorWindow.GetWindow(gvWndType);
			selectedSizeIndexProp.SetValue(gvWnd, index, null);
		}

		public static int FindSize(string text)
		{
			object group = GetGroup();
			MethodInfo getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
			string[] displayTexts = getDisplayTexts.Invoke(group, null) as string[];

			for (int i = 0; i < displayTexts.Length; i++)
			{
				string display = displayTexts[i];
				int pren = display.IndexOf('(');
				if (pren != -1)
					display = display.Substring(0, pren - 1);

				if (display == text)
					return i;
			}

			return -1;
		}

		public static int FindSize(int width, int height)
		{
			object group = GetGroup();
			Type groupType = group.GetType();
			MethodInfo getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
			MethodInfo getCustomCount = groupType.GetMethod("GetCustomCount");
			int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
			MethodInfo getGameViewSize = groupType.GetMethod("GetGameViewSize");
			Type gvsType = getGameViewSize.ReturnType;
			PropertyInfo widthProp = gvsType.GetProperty("width");
			PropertyInfo heightProp = gvsType.GetProperty("height");
			object[] indexValue = new object[1];

			for (int i = 0; i < sizesCount; i++)
			{
				indexValue[0] = i;
				object size = getGameViewSize.Invoke(group, indexValue);
				int sizeWidth = (int)widthProp.GetValue(size, null);
				int sizeHeight = (int)heightProp.GetValue(size, null);
				if (sizeWidth == width && sizeHeight == height)
					return i;
			}

			return -1;
		}

		public static int AddCustomSizeIfMissing(int width, int height, string name)
		{
			int existingIndex = FindSize(name);
			if (existingIndex != -1)
				return existingIndex;

			object group = GetGroup();

			Type gameViewSizeType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameViewSize");
			Type gameViewSizeTypeType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameViewSizeType");

			ConstructorInfo ctor = gameViewSizeType.GetConstructor(new Type[]
			{
				gameViewSizeTypeType, typeof(int), typeof(int), typeof(string)
			});

			object newSize = ctor.Invoke(new object[] { Enum.ToObject(gameViewSizeTypeType, 1), width, height, name });

			MethodInfo addCustomSize = group.GetType().GetMethod("AddCustomSize");
			addCustomSize.Invoke(group, new object[] { newSize });

			return FindSize(name);
		}

		private static object GetGroup()
		{
			PropertyInfo type = GameViewSizesInstance.GetType().GetProperty("currentGroupType");
			object value = type.GetValue(GameViewSizesInstance, null);
			return getGroup.Invoke(GameViewSizesInstance, new object[] { value });
		}
	}
}