using Firebase.Analytics;
using Libs.Analytics;
using Scripts.GameState;
using System;
using System.Linq;
using System.Text;
using Zenject;

namespace Scripts.Analytics
{
    public sealed class ParameterWrapper
    {
        public readonly string Name;
        public string ValueString { get; }
        public long ValueLong { get; }
        public double ValueDouble { get; }
        public int ValueType { get; }

        public ParameterWrapper(string name, string value)
        {
            Name = name;
            ValueString = value;
            ValueType = 1;
        }
        public ParameterWrapper(string name, long value)
        {
            Name = name;
            ValueLong = value;
            ValueType = 2;
        }
        public ParameterWrapper(string name, double value)
        {
            Name = name;
            ValueDouble = value;
            ValueType = 3;
        }

        public static implicit operator Parameter(ParameterWrapper w)
        {
            return w.ValueType switch {
                1 => new Parameter(w.Name, w.ValueString),
                2 => new Parameter(w.Name, w.ValueLong),
                3 => new Parameter(w.Name, w.ValueDouble),
                _ => new Parameter(w.Name, w.ValueString)
            };
        }

        public override string ToString()
        {
            return ValueType switch {
                1 => $"{Name}={ValueString}",
                2 => $"{Name}={ValueLong}",
                3 => $"{Name}={ValueDouble}",
                _ => $"{Name}=?"
            };
        }
    }
    public sealed class FirebaseAnalyticsController : IInitializable, IDisposable
    {
        public const string EVENT_NEW_GAME = "game_new";
        public const string EVENT_RESTART_GAME = "game_restart";
        public const string EVENT_WIN_GAME = "game_win";
        public const string EVENT_LOOSE_GAME = "game_loose";
        public const string EVENT_SHOW_HINT = "game_show_hint";
        public const string EVENT_CANCEL_MOVE = "game_cancel_move";
        public const string PARAMETER_GAME_TYPE = "parameter_game_type";
        public const string PARAMETER_USER_BALL = "parameter_user_ball";
        public const string PARAMETER_GAMES_COUNT = "parameter_games_count";
        public const string PARAMETER_GAME_TIME = "parameter_game_time";

        [Inject]
        IGameStateService _gameState;

        public void Initialize()
        {
            _gameState.OnNewGame += GameState_OnNewGame;
            _gameState.OnWinGame += GameState_OnWinGame;
            _gameState.OnLooseGame += GameState_OnLooseGame;
            _gameState.OnCancelMove += GameState_OnCancelMove;
            _gameState.OnShowHint += GameState_OnShowHint;
        }

        public void Dispose()
        {
            _gameState.OnNewGame -= GameState_OnNewGame;
            _gameState.OnWinGame -= GameState_OnWinGame;
            _gameState.OnLooseGame -= GameState_OnLooseGame;
            _gameState.OnCancelMove -= GameState_OnCancelMove;
            _gameState.OnShowHint -= GameState_OnShowHint;
        }

        void GameState_OnNewGame(bool isRestart, GameType gameType, IGameStateService service)
        {
            return;
        }
        void GameState_OnWinGame(IGameStateService obj)
        {
            return;
        }
        void GameState_OnLooseGame(IGameStateService obj)
        {
            return;
        }
        void GameState_OnShowHint(IGameStateService obj)
        {
            return;
        }
        void GameState_OnCancelMove(IGameStateService obj)
        {
            return;
        }
    }
}