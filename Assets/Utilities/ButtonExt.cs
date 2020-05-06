using UnityEngine.UI;

namespace Utilities.Extensions {
	public static class ButtonExtensions {
		public static void SetNavigation(this Button button, Button up, Button down, Button right, Button left) {
			var nav = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnUp = up,
				selectOnDown = down,
				selectOnLeft = left,
				selectOnRight = right
			};
			button.navigation = nav;
		}
	}
}