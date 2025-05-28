using Scripts.GameEvents;
using System;
using UnityEngine;
using Zenject;
using static Scripts.GameState.IGameStateService;

namespace Scripts.GameState
{
    public sealed class GameStateService : IGameStateService, IInitializable, IDisposable
    {
        const string LAYOUT_ID_PREF = "layout_id";

        [Inject]
        IGameEventsService _gameEvents;

        float _startGameTime;
        float _gameEndTime;
        bool _isPlaying;
        bool _isLooseGame;
        bool _isWinGame;

        public float GameTime => IsPlaying ? Time.unscaledTime - _startGameTime : _gameEndTime - _startGameTime;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (value == _isPlaying) return;
                _isPlaying = value;
                if (!_isPlaying) _gameEndTime = Time.unscaledTime;
            }
        }
        public bool IsLooseGame
        {
            get => _isLooseGame;
            set
            {
                if (value == _isLooseGame) return;
                _isLooseGame = value;
                if (_isLooseGame) {
                    IsPlaying = false;
                    OnLooseGame?.Invoke(this);
                }
            }
        }
        public bool IsWinGame
        {
            get => _isWinGame;
            set
            {
                if (value == _isWinGame) return;
                _isWinGame = value;
                if (_isWinGame) {
                    IsPlaying = false;
                    OnWinGame?.Invoke(this);
                }
            }
        }
        public GameType GameType { get; private set; }
        public int LayoutId
        {
            get => PlayerPrefs.GetInt(LAYOUT_ID_PREF, 0);
            set => PlayerPrefs.SetInt(LAYOUT_ID_PREF, value);
        }

        public void Initialize()
        {
            _gameEvents.OnNewGame += GameEvents_OnNewGame;
            _gameEvents.OnCancelMove += GameEvents_OnCancelMove;
            _gameEvents.OnLooseGame += GameEvents_OnLooseGame;
            _gameEvents.OnRestartGame += GameEvents_OnRestartGame;
            _gameEvents.OnShowHint += GameEvents_OnShowHint;
            _gameEvents.OnShowLooseHint += GameEvents_OnShowLooseHint;
            _gameEvents.OnWinGame += GameEvents_OnWinGame;
        }
        public void Dispose()
        {
            _gameEvents.OnNewGame -= GameEvents_OnNewGame;
            _gameEvents.OnCancelMove -= GameEvents_OnCancelMove;
            _gameEvents.OnLooseGame -= GameEvents_OnLooseGame;
            _gameEvents.OnRestartGame -= GameEvents_OnRestartGame;
            _gameEvents.OnShowHint -= GameEvents_OnShowHint;
            _gameEvents.OnShowLooseHint -= GameEvents_OnShowLooseHint;
            _gameEvents.OnWinGame -= GameEvents_OnWinGame;
        }

        void GameEvents_OnShowLooseHint(IGameEventsService obj)
        {
            IsLooseGame = true;
        }
        void GameEvents_OnShowHint(IGameEventsService obj)
        {
            OnShowHint?.Invoke(this);
        }
        void GameEvents_OnRestartGame(GameType gameType, IGameEventsService obj)
        {
            NewGame(gameType, LayoutId, true);
        }
        void GameEvents_OnLooseGame(IGameEventsService obj)
        {
            IsLooseGame = true;
        }
        void GameEvents_OnCancelMove(IGameEventsService obj)
        {
            OnCancelMove?.Invoke(this);
        }
        void GameEvents_OnNewGame(GameType gameType, int layoutId, IGameEventsService obj)
        {
            NewGame(gameType, layoutId, false);
        }
        void GameEvents_OnWinGame(IGameEventsService obj)
        {
            IsWinGame = true;
        }

        void NewGame(GameType gameType, int layoutId, bool restart)
        {
            OnBeforeNewGame?.Invoke(restart, gameType, this);
            _startGameTime = Time.unscaledTime;
            IsPlaying = true;
            IsLooseGame = false;
            IsWinGame = false;
            GameType = gameType;
            LayoutId = layoutId;
            OnNewGame?.Invoke(restart, gameType, this);
        }

        public event Action<IGameStateService> OnShowHint;
        public event OnNewGameDelegate OnNewGame;
        public event OnNewGameDelegate OnBeforeNewGame;
        public event Action<IGameStateService> OnCancelMove;
        public event Action<IGameStateService> OnLooseGame;
        public event Action<IGameStateService> OnWinGame;
    }
}