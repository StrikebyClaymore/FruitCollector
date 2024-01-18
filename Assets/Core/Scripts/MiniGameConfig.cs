using UnityEngine;

namespace Core.Scripts
{
    [CreateAssetMenu(menuName = "MiniGameConfig", fileName = "MiniGameConfig", order = 51)]
    public class MiniGameConfig : ScriptableObject
    {
        public int BaseIncome;
        public int BonusIncome;
    }
}