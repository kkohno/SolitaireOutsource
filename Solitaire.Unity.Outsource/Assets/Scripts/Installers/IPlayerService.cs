using Scripts.DebugLog;

namespace Scripts.Installers
{
    /// <summary>
    /// проигрыватель игры
    /// </summary>
    [Debug("PlayerService")]
    public interface IPlayerService
    {
        /// <summary>
        /// производит один ход вперед
        /// </summary>
        /// <returns>истина, если ход был сделан</returns>
        [Debug("Next")]
        bool Next();
    }
}