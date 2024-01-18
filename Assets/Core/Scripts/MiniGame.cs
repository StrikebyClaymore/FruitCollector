using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Core.Scripts
{
    public class MiniGame : MonoBehaviour
    {
        private GameDatabase _gameDatabase;
        private MiniGameConfig _config;
        [SerializeField] private TextMeshProUGUI _moneyMesh;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Slot[] _slots;
        private const int TypeAmount = 3;
        private const int MaxCount = 3;
        private Dictionary<EFruits, int> _counter = new Dictionary<EFruits, int>();
        private int _count;
        private int _money;
        public Action<int> OnMiniGameEnded;

        private void Awake()
        {
            _acceptButton.onClick.AddListener(OnAcceptButtonPressed);
        }

        public void Initialize(GameDatabase database)
        {
            _gameDatabase = database;
            _config = database.MiniGameConfig;
            foreach (var slot in _slots)
                slot.OnPressed += SlotPressed;
            SetMoney();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            var unvisitedSlots = new List<Slot>(_slots);
            FillType(EFruits.Pear, ref unvisitedSlots);
            FillType(EFruits.Banana, ref unvisitedSlots);
            FillType(EFruits.Strawberry, ref unvisitedSlots);
        }

        private void Close()
        {
            OnMiniGameEnded?.Invoke(_money);
            _counter.Clear();
            foreach (var slot in _slots)
                slot.Hide();
            _count = 0;
            _money = 0;
            SetMoney();
            gameObject.SetActive(false);
        }
        
        private void FillType(EFruits type, ref List<Slot> unvisitedSlots)
        {
            for (int i = 0; i < TypeAmount; i++)
            {
                var slot = _slots[Random.Range(0, unvisitedSlots.Count)];
                slot.SetFruit(type, _gameDatabase.GetSprite(type));
                unvisitedSlots.Remove(slot);
            }
        }
        
        private void SlotPressed(Slot slot, EFruits type)
        {
            if (_count == MaxCount)
                return;

            slot.Show();
            
            _count++;
            _money += _config.BaseIncome;
            if (_counter.ContainsKey(type))
            {
                _counter[type]++;
                if (_counter[type] == MaxCount)
                    _money += _config.BonusIncome;
            }
            else
            {
                _counter[type] = 1;   
            }

            SetMoney();
        }

        private void SetMoney()
        {
            _moneyMesh.text = $"{_money}$";
        }
        
        private void OnAcceptButtonPressed()
        {
            Close();
        }
    }
}