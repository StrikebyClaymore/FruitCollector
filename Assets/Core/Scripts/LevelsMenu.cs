using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Scripts
{
    public class LevelsMenu : MonoBehaviour
    {
        [SerializeField] private LevelElement _levelElementPrefab;
        [SerializeField] private Transform _levelElementsContainer;
        [SerializeField] private TextMeshProUGUI _moneyValueMesh;

        public void Build(GameDatabase gameDatabase)
        {
            var childCount = _levelElementsContainer.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = _levelElementsContainer.GetChild(i);
                DestroyImmediate(child.gameObject);
            }

            for (var i = 0; i < gameDatabase.Levels.Length; i++)
            {
                var levelData = gameDatabase.Levels[i];
                var levelElement = Instantiate(_levelElementPrefab, _levelElementsContainer);
                levelElement.SetText(i, levelData.Price);
                if (levelData.Price == 0)
                    levelElement.Unlock();
            }
        }

        public void Initialize(GameLogic gameLogic)
        {
            for (int i = 0; i < _levelElementsContainer.childCount; i++)
            {
                var child = _levelElementsContainer.GetChild(i);
                if (child.TryGetComponent<LevelElement>(out var levelElement))
                {
                    levelElement.Connect(gameLogic, i);                    
                }
            }
        }

        public void SetMoney(int money)
        {
            _moneyValueMesh.text = money.ToString();
        }
    }
}