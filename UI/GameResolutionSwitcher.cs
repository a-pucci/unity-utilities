using System;
using UnityEngine;

namespace AP.Utilities.UI
{
    public class GameResolutionSwitcher : MonoBehaviour
    {
        [Serializable]
        public struct GameResData
        {
            [field: SerializeField]
            public GameObject Reference { get; private set; }
            public string ResolutionName => Reference.name;
            [field: SerializeField]
            public Vector2Int ResolutionSize { get; private set; }
        }

        [field: SerializeField]
        public GameResData[] Landscape { get; private set; }
        [field: SerializeField]
        public GameResData[] Portrait { get; private set; }
    }
}