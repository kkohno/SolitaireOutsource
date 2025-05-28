using System.Linq;

namespace Scripts.Decks
{
    public static class LayoutConvertExtensions
    {
        /// <summary>
        /// получает входные данные для игрового контроллера юнити из данных расклада 
        /// </summary>
        /// <param name="layout">расклад</param>
        /// <returns>данные, пригодные, для употребления движком, написанным на юнити</returns>
       /* public static int[] ToGameControllerDataFormat(this Layout layout)
        {
            return layout.Data.Select(i => (int)i).ToArray();
        }*/
    }
}