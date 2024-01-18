using System;
using UnityEngine;

namespace Core.Scripts
{
    [CreateAssetMenu(menuName = "GameDatabase", fileName = "GameDatabase", order = 51)]
    public class GameDatabase : ScriptableObject
    {
        [SerializeField] private Sprite[] _sprites;
        public LevelConfig[] Levels;
        public MiniGameConfig MiniGameConfig;

        public Sprite GetSprite(EFruits type)
        {
            return _sprites[(int)type];
        }
    }
}