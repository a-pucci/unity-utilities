using UnityEngine;
using UnityEditor;

namespace AP.Utilities.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(Range01Attribute))]
	public class Range01Drawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var range = (Range01Attribute)attribute;

			EditorGUI.BeginProperty(position, label, property);

			float labelWidth = EditorGUIUtility.labelWidth;
			float fieldWidth = 50f;
			float prefixWidth = string.IsNullOrEmpty(range.PrefixLabel) ? 0 : GUI.skin.label.CalcSize(new GUIContent(range.PrefixLabel)).x + 5;
			float suffixWidth = string.IsNullOrEmpty(range.SuffixLabel) ? 0 : GUI.skin.label.CalcSize(new GUIContent(range.SuffixLabel)).x + 5;

			var labelRect = new Rect(position.x, position.y, labelWidth, position.height);
			EditorGUI.LabelField(labelRect, label);

			float contentX = position.x + labelWidth;
			float contentWidth = position.width - labelWidth;

			// Prefix
			float prefixValue = property.floatValue;

			if (!string.IsNullOrEmpty(range.PrefixLabel))
			{
				var prefixLabelRect = new Rect(contentX, position.y, prefixWidth, position.height);
				EditorGUI.LabelField(prefixLabelRect, range.PrefixLabel);
				contentX += prefixWidth;
				contentWidth -= prefixWidth;

				var prefixFieldRect = new Rect(contentX, position.y, fieldWidth, position.height);
				EditorGUI.BeginChangeCheck();
				float newPrefixValue = EditorGUI.FloatField(prefixFieldRect, prefixValue);
				if (EditorGUI.EndChangeCheck())
					property.floatValue = newPrefixValue;

				contentX += fieldWidth + 5;
				contentWidth -= fieldWidth + 5;
			}

			float suffixFieldWidth = fieldWidth;
			if (!string.IsNullOrEmpty(range.SuffixLabel))
				contentWidth -= suffixWidth + suffixFieldWidth + 5;


			// Slider
			var sliderRect = new Rect(contentX, position.y, contentWidth, position.height);
			EditorGUI.BeginChangeCheck();
			int oldIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			float newValue = GUI.HorizontalSlider(sliderRect, property.floatValue, 0f, 1f);

			if (EditorGUI.EndChangeCheck())
				property.floatValue = newValue;

			EditorGUI.indentLevel = oldIndent;

			// Suffix
			if (!string.IsNullOrEmpty(range.SuffixLabel))
			{
				contentX += contentWidth + 5;

				var suffixLabelRect = new Rect(contentX, position.y, suffixWidth, position.height);
				EditorGUI.LabelField(suffixLabelRect, range.SuffixLabel);
				contentX += suffixWidth;

				var suffixFieldRect = new Rect(contentX, position.y, suffixFieldWidth, position.height);
				EditorGUI.BeginChangeCheck();
				float suffixValue = 1 - property.floatValue;
				float newSuffixValue = EditorGUI.FloatField(suffixFieldRect, suffixValue);

				if (EditorGUI.EndChangeCheck())
					property.floatValue = 1 - newSuffixValue;
			}

			EditorGUI.EndProperty();
		}
	}
}