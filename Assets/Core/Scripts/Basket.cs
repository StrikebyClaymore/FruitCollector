using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts
{
    public class Basket : MonoBehaviour
    {
        [SerializeField] private Image _selecting;
        [SerializeField] private EFruits _type;
        public EFruits Type => _type;
        [SerializeField] private Button _button;
        public Action<Basket> OnPressed;

        private void Awake()
        {
            _button.onClick.AddListener(OnButtonPressed);
        }

        public void Select(bool enable)
        {
            _selecting.enabled = enable;
        }
        
        private void OnButtonPressed()
        {
            OnPressed?.Invoke(this);
        }
    }
}