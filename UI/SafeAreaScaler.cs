using UnityEngine;

namespace AP.Utilities.UI
{
    [RequireComponent(typeof(Canvas))]

    public class SafeAreaScaler : MonoBehaviour
    {
        private Canvas canvas;
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Interpolation between Screen width and Safe Area width." +
                 "\nIf 0, UI width will match the Screen width." +
                 "\nIf 1, UI width will match to the Safe Area width.")]
        private float matchX = 1;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Interpolation between Screen height and Safe Area height." +
                 "\nIf 0, UI height will match the Screen height." +
                 "\nIf 1, UI height will match to Safe Area height.")]
        private float matchY = 1;

        private static Rect safeArea;

#if UNITY_SWITCH
        private const float CheckRate = 1f;
        private float elapsedTime = 0;
#endif

        private void Awake() => canvas = GetComponent<Canvas>();

        private void Start() => ApplySafeArea();

        private void ApplySafeArea()
        {
            if (safeArea == default)
                safeArea = Screen.safeArea;

            var safeAreaTransform = transform as RectTransform;
            if (safeAreaTransform == null)
                return;

            Rect canvasRect = canvas.pixelRect;
            if (canvasRect.width <= 0 || canvasRect.height <= 0)
                return;

            var safeAnchorMin = new Vector2(safeArea.xMin / canvasRect.width, safeArea.yMin / canvasRect.height);
            var safeAnchorMax = new Vector2(safeArea.xMax / canvasRect.width, safeArea.yMax / canvasRect.height);

            Vector2 screenAnchorMin = Vector2.zero;
            Vector2 screenAnchorMax = Vector2.one;

            var anchorMin = new Vector2(
                Mathf.Lerp(screenAnchorMin.x, safeAnchorMin.x, matchX),
#if UNITY_IOS
            0
#else
                Mathf.Lerp(screenAnchorMin.y, safeAnchorMin.y, matchY)
#endif
            );

            var anchorMax = new Vector2(
                Mathf.Lerp(screenAnchorMax.x, safeAnchorMax.x, matchX),
                Mathf.Lerp(screenAnchorMax.y, safeAnchorMax.y, matchY)
            );

            safeAreaTransform.anchorMin = anchorMin;
            safeAreaTransform.anchorMax = anchorMax;
        }

#if UNITY_SWITCH
        private void Update() => CheckSafeArea();

        private void CheckSafeArea()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime < CheckRate)
                return;

            elapsedTime -= CheckRate;

            if (safeArea != Screen.safeArea)
            {
                safeArea = Screen.safeArea;
                ApplySafeArea();
            }

        }
#endif
    }
}