using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.Scripts
{
    public class LevelElement : MonoBehaviour
    {
        [SerializeField] private Button _levelButton;
        [SerializeField] private TextMeshProUGUI _levelTextMesh;
        [SerializeField] private Button _lockButton;
        [SerializeField] private TextMeshProUGUI _costTextMesh;
        private int _level;
        private Action<int> _onLevelPressed;
        private Action<LevelElement, int> _onLockPressed;

        public void SetText(int level, int cost)
        {
            _levelTextMesh.text = $"Level {level + 1}";
            _costTextMesh.text = $"{cost}$";
        }

        public void Unlock()
        {
            _lockButton.gameObject.SetActive(false);
            _costTextMesh.gameObject.SetActive(false);
        }

        public void Connect(GameLogic gameLogic, int level)
        {
            _level = level;
            _onLevelPressed = gameLogic.SelectLevel;
            _onLockPressed = gameLogic.TryBuyLevel;
            _levelButton.onClick.AddListener(OnLevelButtonPressed);
            _lockButton.onClick.AddListener(OnLockButtonPressed);
        }

        private void OnLevelButtonPressed()
        {
            if(_lockButton.gameObject.activeSelf)
                return;
            _onLevelPressed?.Invoke(_level);
        }
        
        private void OnLockButtonPressed()
        {
            _onLockPressed?.Invoke(this, _level);
        }
    }
}