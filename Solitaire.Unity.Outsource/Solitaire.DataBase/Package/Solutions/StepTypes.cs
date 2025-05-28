namespace Solitaire.DataBase.Solutions
{
    /// <summary>
    /// типы возможных шагов
    /// </summary>
    public enum StepTypes
    {
        /// <summary>
        /// перемещение карты
        /// </summary>
        MoveCards,
        /// <summary>
        /// перемещение колонки целиком
        /// </summary>
        MoveWholeTableau,
        /// <summary>
        /// перемещение в дом
        /// </summary>
        MoveToFoundation,
        /// <summary>
        /// перемещение карты из открытых карт
        /// </summary>
        MoveFromWaste,
        /// <summary>
        /// перемещение карты из открытых карт в дом
        /// </summary>
        MoveFromWasteToFoundation,
        /// <summary>
        /// перемещение из дома
        /// </summary>
        MoveFromFoundation,
        /// <summary>
        /// открыть карту в колоде
        /// </summary>
        DrawFromStock
    }
}