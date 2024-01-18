using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Scripts
{
    [CreateAssetMenu(menuName = "LevelConfig", fileName = "LevelConfig", order = 51)]
    public class LevelConfig : ScriptableObject
    {
        [Serializable]
        public class LevelObj
        {
            public EFruits Type;
            public Vector3Int Position;
        }

        public Vector2Int MapSize;
        public LevelObj[] Map;
        public Vector3Int TractorStartPosition = Vector3Int.zero;
        public Vector2Int TractorStartDirection = Vector2Int.up;
        public int Time;
        public int Income;
        public int Price;
    }
}