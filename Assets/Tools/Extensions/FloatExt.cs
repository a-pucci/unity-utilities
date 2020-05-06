using UnityEngine;

namespace Tools.Extensions {
	public static class FloatExt {
		public static float Remap(this float v, float iMin, float iMax, float oMin, float oMax) {
			float t = Mathf.InverseLerp(iMin, iMax, v);
			return Mathf.Lerp(oMin, oMax, t);
		}
	}
}