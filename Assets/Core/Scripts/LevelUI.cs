using TMPro;
using UnityEngine;

namespace Core.Scripts
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelMesh;
        [SerializeField] private TextMeshProUGUI _timeMesh;
        [SerializeField] private TextMeshProUGUI _progressMesh;

        public void SetTime(float timeLeft)
        {
            _timeMesh.text = Mathf.Ceil(timeLeft).ToString();
        }

        public void SetProgress(int fruitsCount, int maxFruitsCount)
        {
            _progressMesh.text = $"{fruitsCount}/{maxFruitsCount}";
        }

        public void SetLevel(int level)
        {
            _levelMesh.text = $"Level {level + 1}";
        }
    }
}