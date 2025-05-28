using Scripts.DecksDataBase.Services;
using System.Threading.Tasks;
using Zenject;

namespace Scripts.Decks
{
    /// <summary>
    /// сервис выбора расклада из базы данных раскладов
    /// </summary>
    public sealed class DecksService : IDecksService
    {
        [Inject]
        IDecksDataBaseService _decksDataBase;

    }
}