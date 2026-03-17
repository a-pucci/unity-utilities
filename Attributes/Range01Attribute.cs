using UnityEngine;

namespace AP.Utilities.Attributes
{
	public class Range01Attribute : PropertyAttribute
	{
		public string PrefixLabel { get; private set; }
		public string SuffixLabel { get; private set; }

		public Range01Attribute(string prefixLabel = "", string suffixLabel = "")
		{
			PrefixLabel = prefixLabel;
			SuffixLabel = suffixLabel;
		}
	}
}