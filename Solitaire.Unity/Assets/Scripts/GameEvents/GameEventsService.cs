using System;

namespace Scripts.GameEvents
{
    public sealed class GameEventsService : IGameEventsService
    {
        public void ShowHint()
        {
            OnShowHint?.Invoke(this);
        }
        public void ShowLooseHint()
        {
            OnShowLooseHint?.Invoke(this);
        }
        public void NewGame(GameType gameType, int layoutId)
        {
            OnNewGame?.Invoke(gameType, layoutId, this);
        }
        public void RestartGame(GameType gameType)
        {
            OnRestartGame?.Invoke(gameType, this);
        }
        public void CancelMove()
        {
            OnCancelMove?.Invoke(this);
        }
        public void LooseGame()
        {
            OnLooseGame?.Invoke(this);
        }
        public void WinGame()
        {
            OnWinGame?.Invoke(this);
        }

        public event Action<IGameEventsService> OnShowHint;
        public event Action<IGameEventsService> OnShowLooseHint;
        public event Action<GameType, int, IGameEventsService> OnNewGame;
        public event Action<GameType, IGameEventsService> OnRestartGame;
        public event Action<IGameEventsService> OnCancelMove;
        public event Action<IGameEventsService> OnLooseGame;
        public event Action<IGameEventsService> OnWinGame;
    }
}