using System.Text;

namespace Scripts.Debuging
{
    public static class DebugExtensions
    {
        public static string Dump(this Card[] cards)
        {
            var sb = new StringBuilder("[");
            foreach (var c in cards) sb.Append($"{c.Dump()}; ");
            sb.Append(']');
            return sb.ToString();
        }
        public static string Dump(this Card card)
        {
            return card == null ? "<NULL>" : $"{card.Value}";
        }
        public static string Dump(this int[] values)
        {
            var sb = new StringBuilder("[");
            foreach (var c in values) sb.Append($"{c}; ");
            sb.Append(']');
            return sb.ToString();
        }
    }
}