using UnityEngine;

namespace Tools.Extensions {
	public static class Vector3Ext {
		public static Vector3 OppositeDirection(this Vector3 vector) {
			return new Vector3(-vector.x, vector.y, -vector.z).normalized;
		}

		public static Vector3 PerpendicularClockwise(this Vector3 vector) {
			return new Vector3(vector.z, 0, -vector.x);
		}

		public static Vector3 PerpendicularCounterClockwise(this Vector3 vector) {
			return new Vector3(-vector.z, 0, vector.x);
		}
	}
}