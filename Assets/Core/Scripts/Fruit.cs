using System;
using UnityEngine;

namespace Core.Scripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Fruit : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        public EFruits Type;
        public Sprite Sprite
        {
            get => _renderer.sprite;
            set => _renderer.sprite = value;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

#endif
        
    }
}