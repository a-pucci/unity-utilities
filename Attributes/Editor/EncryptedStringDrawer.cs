using UnityEditor;
using UnityEngine;

namespace AP.Utilities.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(EncryptedStringAttribute))]
	public class EncryptedStringDrawer : PropertyDrawer
	{
		private const int Lines = 2;
		private const float Padding = 2f;
		private const float ButtonWidth = 60f;
		private string tempValue;
		private static Color RectColor => new(0.5f, 0.5f, 0.5f, 0.5f);
		private static Color DecryptedColor => new(1f, 0f, 0f, 0.5f);
		private static float LineHeight => EditorGUIUtility.singleLineHeight;

		private EncryptedStringAttribute Attribute => (EncryptedStringAttribute)attribute;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String)
			{
				EditorGUI.LabelField(position, "Warning: Use the attribute [EncryptedString] only on a string field");
				return;
			}

			EditorGUI.BeginProperty(position, label, property);

			DrawBackground(position);

			DrawLabel(position, label);

			DrawDecryptedValue(position, property);

			DrawInputField(position, property);

			property.serializedObject.ApplyModifiedProperties();

			EditorGUI.EndProperty();
		}

		private static void DrawBackground(Rect position)
		{
			EditorGUI.DrawRect(
				new Rect(new Vector2(position.position.x - Padding, position.position.y),
					new Vector2(position.width + Padding * 2, position.height)), RectColor);
		}

		private static void DrawLabel(Rect position, GUIContent label)
		{
			var pos = new Vector2(position.position.x, position.position.y);

			EditorGUI.LabelField(new Rect(pos, new Vector2(position.width, position.height)), label);
			pos.y += Padding;

			GUI.enabled = false;
			GUI.backgroundColor = DecryptedColor;
		}

		private void DrawDecryptedValue(Rect position, SerializedProperty property)
		{
			float fieldWidth = position.width - ButtonWidth - Padding;
			var pos = new Vector2(position.x, position.y + Padding);
			string clearValue = string.IsNullOrEmpty(property.stringValue) ? "" : Attribute.Decrypt(property.stringValue);
			EditorGUI.TextField(new Rect(pos, new Vector2(fieldWidth, LineHeight)), " ", clearValue);

			GUI.enabled = true;
			GUI.backgroundColor = Color.white;

			if (GUI.Button(new Rect(pos.x + position.width - ButtonWidth, pos.y, ButtonWidth, LineHeight), "Copy"))
				GUIUtility.systemCopyBuffer = clearValue;
		}

		private void DrawInputField(Rect position, SerializedProperty property)
		{
			float fieldWidth = position.width - ButtonWidth - Padding;
			var pos = new Vector2(position.x, position.y + LineHeight + Padding * 2);

			tempValue = EditorGUI.TextField(new Rect(pos, new Vector2(fieldWidth, LineHeight)), " ", tempValue);

			if (GUI.Button(new Rect(pos.x + position.width - ButtonWidth, pos.y, ButtonWidth, LineHeight), "Set"))
			{
				property.stringValue = Attribute.Encrypt(tempValue);
				tempValue = "";
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight * Lines + Padding * (Lines + 1);
		}
	}
}