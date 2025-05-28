using SQLite;

namespace Solitaire.DataBase
{
    [SQLite.Table("solutions")]
    public sealed class Solution
    {
        [SQLite.Column("layout_id"), PrimaryKey, Unique]
        public int LayoutId { get; set; }
        [SQLite.Column("steps_count"), NotNull]
        public int StepsCount { get; set; }
        [SQLite.Column("data"), NotNull]
        public byte[] Data { get; set; }
    }
}