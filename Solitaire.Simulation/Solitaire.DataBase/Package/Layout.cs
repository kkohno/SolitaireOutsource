using SQLite;

namespace Solitaire.DataBase
{
    [SQLite.Table("layouts")]
    public sealed class Layout
    {
        [SQLite.Column("id"), PrimaryKey, AutoIncrement, Unique]
        public int Id { get; set; }
        [SQLite.Column("visual"), NotNull]
        public string Visual { get; set; }
        /// <summary>
        /// сложность, причем 0 это не задано и далее по нарастающей упрощается сложность , тоесть 1 это самое сложное
        /// </summary>
        [SQLite.Column("complexity")]
        public byte Complexity { get; set; }
        [SQLite.Column("solutions_count"), NotNull]
        public int SolutionsCount { get; set; }
        [SQLite.Column("type"), NotNull]
        public byte Type { get; set; }
        [SQLite.Column("data"), NotNull, MaxLength(52)]
        public byte[] Data { get; set; }
        [SQLite.Column("collection")]
        public int Collection { get; set; }
    }
}