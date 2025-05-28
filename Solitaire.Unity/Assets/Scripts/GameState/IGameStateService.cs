using System;

namespace Scripts.GameState
{
    /// <summary>
    /// логические эвенты игры и его состояние
    /// </summary>
    public interface IGameStateService
    {
        /// <summary>
        /// время от начала игры в секундах
        /// <para>когда игра окончена то замораживается</para>
        /// </summary>
        float GameTime { get; }
        /// <summary>
        /// продолжается ли игра или уже завершена
        /// </summary>
        bool IsPlaying { get; }
        /// <summary>
        /// игра проиграна
        /// </summary>
        bool IsLooseGame { get; }
        /// <summary>
        /// игра выиграна
        /// </summary>
        bool IsWinGame { get; }
        /// <summary>
        /// текущий тип игры
        /// </summary>
        GameType GameType { get; }
        /// <summary>
        /// текущий ID расклада или 0, если id не известен
        /// </summary>
        int LayoutId { get; }

        /// <summary>
        /// взята подсказка
        /// </summary>
        event Action<IGameStateService> OnShowHint;
        /// <summary>
        /// новая игра
        /// </summary>
        event OnNewGameDelegate OnNewGame;
        /// <summary>
        /// до смены состояния новой игры
        /// </summary>
        event OnNewGameDelegate OnBeforeNewGame;
        delegate void OnNewGameDelegate(bool isRestart, GameType gameType, IGameStateService service);
        /// <summary>
        /// ход отменен
        /// </summary>
        event Action<IGameStateService> OnCancelMove;
        /// <summary>
        /// игра проиграна
        /// </summary>
        event Action<IGameStateService> OnLooseGame;
        /// <summary>
        /// игра выиграна
        /// </summary>
        event Action<IGameStateService> OnWinGame;
    }
}