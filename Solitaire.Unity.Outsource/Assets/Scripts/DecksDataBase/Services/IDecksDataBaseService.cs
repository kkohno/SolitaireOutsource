using System.Threading.Tasks;

namespace Scripts.DecksDataBase.Services
{
    /// <summary>
    /// сервис раскладов карт (база данных краскладов)
    /// </summary>
    public interface IDecksDataBaseService
    {
        //Task<Layout> GetDeck(GameType type);

        /// <summary>
        /// возвращает решение на расклад по его ID
        /// </summary>
        /// <param name="layoutId">ID расклада</param>
       // Task<Solution> GetSolution(int layoutId);
    }
}