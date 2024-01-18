using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Core.Scripts
{
    public class GameLogic : MonoBehaviour
    {
        [SerializeField] private GameDatabase _gameDatabase;
        [SerializeField] private LevelsMenu _levelsMenu;
        [SerializeField] private LevelUI _levelUI;
        [SerializeField] private MiniGame _miniGame;
        [SerializeField] private int _currentLevelIndex;
        private LevelConfig CurrentLevel => _gameDatabase.Levels[_currentLevelIndex];
        [SerializeField] private Basket _basket1;
        [SerializeField] private Basket _basket2;
        private Basket _selectedBasket;
        [SerializeField] private Transform _tractor;
        private Timer _moveTimer;
        [SerializeField] private float _moveInterval = 0.4f;
        [SerializeField] private float _moveDuration = 0.3f;
        [SerializeField] private Vector2Int _moveDirection;
        private Vector3Int _tractorPosition;
        private Coroutine _tractorMoveCoroutine;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TileBase _tile;
        private Tile[,] _map;
        [SerializeField] private Fruit _fruitPrefab;
        [SerializeField, SerializeReference] private List<Fruit> _fruits = new List<Fruit>();
        private int _maxFruitsCount;
        private int _fruitsCount;
        private int _money = 150;
        private Timer _gameLimitTimer;
        private bool _isPaused;
        [SerializeField] private float _cameraHeight;
        [SerializeField] private float _desiredAspect;
        
        private void Awake()
        {
            _isPaused = true;
            _levelsMenu.Initialize(this);
            _levelsMenu.SetMoney(_money);
            _miniGame.Initialize(_gameDatabase);
            _miniGame.OnMiniGameEnded += MiniGameEnded;
            _basket1.OnPressed += SelectBasket;
            _basket2.OnPressed += SelectBasket;
            _moveTimer = new Timer(_moveInterval, OnMoveTimerTimeOut, true);
            _gameLimitTimer = new Timer();
            _gameLimitTimer.OnUpdate += OnGameTimerUpdated;
            _gameLimitTimer.OnCompleted += CloseLevel;
            SetupCamera();
        }

        private void Start()
        {
            InputsHandler.Instance.OnDirectionSwiped += SetTractorDirection;
            _levelUI.gameObject.SetActive(false);
            _miniGame.gameObject.SetActive(false);
            _levelsMenu.gameObject.SetActive(true);
        }

        private void Update()
        {
            _gameLimitTimer.Update();
            _moveTimer.Update();
        }

        public void SelectLevel(int index)
        {
            _currentLevelIndex = index;
            BuildLevel();
            SelectBasket(_basket1);
            _gameLimitTimer.Time = CurrentLevel.Time;
            _levelUI.SetTime(CurrentLevel.Time);
            _levelsMenu.gameObject.SetActive(false);
            _levelUI.gameObject.SetActive(true);
            _isPaused = false;
            _moveDirection = CurrentLevel.TractorStartDirection;
            _gameLimitTimer.Enable(true);
            _moveTimer.Enable(true);
        }
        
        public void TryBuyLevel(LevelElement levelElement, int index)
        {
            if (_money < CurrentLevel.Price)
                return;
            _money -= CurrentLevel.Price;
            _levelsMenu.SetMoney(_money);
            levelElement.Unlock();
        }
        
        [ContextMenu("BuildLevel")]
        private void BuildLevel()
        {
            foreach (var fruit in _fruits.Where(fruit => fruit))
            {
                DestroyImmediate(fruit.gameObject);
            }
            
            _fruits.Clear();
            _tilemap.ClearAllTiles();
            _map = new Tile[CurrentLevel.MapSize.y, CurrentLevel.MapSize.x];
            for (int y = 0; y < CurrentLevel.MapSize.y; y++)
            {
                for (int x = 0; x < CurrentLevel.MapSize.x; x++)
                {
                    var tilePos = new Vector3Int(x, y, 0);
                    _tilemap.SetTile(tilePos, _tile);
                    var tile = new Tile(tilePos);
                    var levelObj = CurrentLevel.Map.FirstOrDefault(e => e.Position == tilePos);
                    if (levelObj != null)
                    {
                        var fruit = Instantiate(_fruitPrefab);
                        fruit.Type = levelObj.Type;
                        fruit.Sprite = _gameDatabase.GetSprite(levelObj.Type);
                        fruit.transform.position = _tilemap.GetCellCenterWorld(tile.Position);
                        tile.SetFruit(fruit);
                        _fruits.Add(fruit);
                    }
                    _map[y, x] = tile;
                }   
            }

            _tractorPosition = CurrentLevel.TractorStartPosition;
            _tractor.transform.position = _tilemap.GetCellCenterWorld(CurrentLevel.TractorStartPosition);
            _maxFruitsCount = _fruits.Count;
            _fruitsCount = 0;
            _levelUI.SetProgress(_fruitsCount, _maxFruitsCount);
            _levelUI.SetLevel(_currentLevelIndex);

            GC.Collect();
        }

        [ContextMenu("BuildUI")]
        private void BuildUI()
        {
            _levelsMenu.Build(_gameDatabase);
        }
        
        private void SelectBasket(Basket basket)
        {
            if(_selectedBasket == basket)
                return;
            if(_selectedBasket)
                _selectedBasket.Select(false);
            _selectedBasket = basket;
            _selectedBasket.Select(true);
        }

        private void SetTractorDirection(Vector2Int direction)
        {
            _moveDirection = (Vector2Int)ScreenDirectionToGame(direction);
        }
        
        private void OnMoveTimerTimeOut()
        {
            MoveTractor();
        }
        
        private void MoveTractor()
        {
            if(_isPaused || _tractorMoveCoroutine != null)
                return;
            var nextTilePos = _tractorPosition + (Vector3Int)_moveDirection;
            if(OutOfBounds(nextTilePos))
                return;
            var nextPosition = _tilemap.GetCellCenterWorld(nextTilePos);
            _tractorMoveCoroutine = StartCoroutine(SmoothTractorMove(_tractor.transform.position, nextPosition, _moveDuration));
            _tractorPosition = nextTilePos;
        }

        private IEnumerator SmoothTractorMove(Vector3 start, Vector3 end, float duration)
        {
            var t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / duration;
                _tractor.transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
            _tractorMoveCoroutine = null;
            TryCollectFruit();
        }

        private bool OutOfBounds(Vector3Int position)
        {
            return _tilemap.GetTile(position) == null;
        }
        
        private void TryCollectFruit()
        {
            var tile = _map[_tractorPosition.y, _tractorPosition.x];
            var fruit = tile.Fruit;
            if (fruit == null)
                return;

            if (fruit.Type != _selectedBasket.Type)
            {
                CloseLevel();
                return;
            }
            
            tile.SetFruit(null);
            _fruits.Remove(fruit);
            Destroy(fruit.gameObject);
            _fruitsCount++;
            _levelUI.SetProgress(_fruitsCount, _maxFruitsCount);
            if(_fruitsCount == _maxFruitsCount)
                LevelCompleted();
        }

        private Vector3Int ScreenDirectionToGame(Vector2Int screenDirection)
        {
            Vector3Int gameDirection = Vector3Int.zero;
            if (screenDirection == Vector2Int.up)
                gameDirection = Vector3Int.right;
            else if(screenDirection == Vector2Int.down)
                gameDirection = Vector3Int.left;
            else if(screenDirection == Vector2Int.right)
                gameDirection = Vector3Int.down;
            else if (screenDirection == Vector2Int.left)
                gameDirection = Vector3Int.up;
            return gameDirection;
        }

        private void LevelCompleted()
        {
            _money += CurrentLevel.Income;
            _levelsMenu.SetMoney(_money);
            CloseLevel();
            _miniGame.Open();
        }

        private void CloseLevel()
        {
            _isPaused = true;
            _gameLimitTimer.Disable();
            _moveTimer.Disable();
            _levelUI.gameObject.SetActive(false);
            _levelsMenu.gameObject.SetActive(true); 
        }
        
        private void MiniGameEnded(int income)
        {
            _money += income;
            _levelsMenu.SetMoney(_money);
        }
        
        private void OnGameTimerUpdated()
        {
            _levelUI.SetTime(_gameLimitTimer.TimeLeft);
        }

        private void SetupCamera()
        {
            var camera = Camera.main;
            float aspect = camera.aspect;
            float ratio =  _desiredAspect / aspect;
            camera.orthographicSize = _cameraHeight * ratio;
        }
        
#if UNITY_EDITOR
        
        private IEnumerator Highlight()
        {
            var bounds = _tilemap.cellBounds;
            var minX = bounds.xMin;
            var maxX = bounds.xMax;
            var minY = bounds.yMin;
            var maxY = bounds.yMax;

            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    Debug.Log($"{_tilemap.GetTile(pos) != null} {pos}");
                    if (_tilemap.GetTile(pos) == null)
                        continue;
                    _tilemap.SetTileFlags(pos, TileFlags.None);
                    _tilemap.SetColor(pos, Color.red);
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        #endif
    }
}