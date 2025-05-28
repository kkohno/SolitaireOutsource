using Scripts.DebugLog;
using System;

namespace Scripts.GameEvents
{
    /// <summary>
    /// сервис всех игровых эвентов
    /// </summary>
    [Debug("IGameEventsService")]
    public interface IGameEventsService
    {
        /// <summary>
        /// взята подсказка
        /// </summary>
        [Debug("ShowHint")]
        void ShowHint();
        /// <summary>
        /// подсказка о проигрыше
        /// </summary>
        [Debug("ShowLooseHint")]
        void ShowLooseHint();
        /// <summary>
        /// новая игра
        /// </summary>
        void NewGame(GameType gameType, int layoutId);
        /// <summary>
        /// перезапуск игры
        /// </summary>
        [Debug("RestartGame")]
        void RestartGame(GameType gameType);
        /// <summary>
        /// ход отменен
        /// </summary>
        [Debug("CancelMove")]
        void CancelMove();
        /// <summary>
        /// игра проиграна
        /// </summary>
        [Debug("LooseGame")]
        void LooseGame();
        /// <summary>
        /// игра выиграна
        /// </summary>
        [Debug("WinGame")]
        void WinGame();

        /// <summary>
        /// взята подсказка
        /// </summary>
        event Action<IGameEventsService> OnShowHint;
        /// <summary>
        /// подсказка о проигрыше
        /// </summary>
        event Action<IGameEventsService> OnShowLooseHint;
        /// <summary>
        /// новая игра
        /// </summary>
        event Action<GameType, int, IGameEventsService> OnNewGame;
        /// <summary>
        /// перезапуск игры
        /// </summary>
        event Action<GameType, IGameEventsService> OnRestartGame;
        /// <summary>
        /// ход отменен
        /// </summary>
        event Action<IGameEventsService> OnCancelMove;
        /// <summary>
        /// игра проиграна
        /// </summary>
        event Action<IGameEventsService> OnLooseGame;
        /// <summary>
        /// игра выиграна
        /// </summary>
        event Action<IGameEventsService> OnWinGame;
    }
}