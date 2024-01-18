using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private Image _locked;
        private EFruits _type;
        public Action<Slot, EFruits> OnPressed;

        private void Awake()
        {
            _button.onClick.AddListener(OnButtonPressed);
        }

        public void SetFruit(EFruits fruit, Sprite sprite)
        {
            _type = fruit;
            _image.sprite = sprite;
        }

        public void Hide()
        {
            _locked.gameObject.SetActive(true);
        }

        public void Show()
        {
            _locked.gameObject.SetActive(false);
        }
        
        private void OnButtonPressed()
        {
            OnPressed?.Invoke(this, _type);
        }
    }
}