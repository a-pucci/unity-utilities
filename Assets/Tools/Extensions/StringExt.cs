namespace Tools.Extensions {
	public static class StringExt {
		public static string UppercaseFirst(this string s) {
			if (string.IsNullOrEmpty(s)) {
				return string.Empty;
			}
			return char.ToUpper(s[0]) + s.Substring(1);
		}

		public static string SnakeCaseToCapitalizedCase(this string s) {
			if (string.IsNullOrEmpty(s)) {
				return string.Empty;
			}
			string[] sA = s.Split('_');
			for (int i = 0; i < sA.Length; i++) {
				sA[i] = sA[i].UppercaseFirst();
			}
			return string.Join(" ", sA);
		}

		public static string SnakeCaseToUpperCase(this string s) {
			if (string.IsNullOrEmpty(s)) {
				return string.Empty;
			}
			string[] sA = s.Split('_');
			for (int i = 0; i < sA.Length; i++) {
				sA[i] = sA[i].ToUpper();
			}
			return string.Join(" ", sA);
		}
	}
}