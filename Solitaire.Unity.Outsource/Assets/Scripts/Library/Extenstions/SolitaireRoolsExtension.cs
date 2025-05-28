using System;
using ui.UIWindowColor;
using UnityEngine;

namespace Scripts.Library.Extenstions
{
    public static class SolitaireRoolsExtension
    {
        /// <summary>
        /// можно ли положить одну карту на другую
        /// </summary>
        /// <param name="down">нижняя карта</param>
        /// <param name="up">верхняя карта</param>
        /// <returns>истина, если ложится</returns>
        public static bool CanStack(Card down, Card up)
        {
            if (down == null) return true;
            if (down.ValueNormalized() != up.ValueNormalized() + 1) return false;
            return down.Suit.IsRed() && up.Suit.IsBlack() || down.Suit.IsBlack() && up.Suit.IsRed();
        }

        /// <summary>
        /// можно ли положить одну карту на другую
        /// <para>выдает ошибку, если ложить нельзя</para>
        /// </summary>
        /// <param name="down">нижняя карта</param>
        /// <param name="up">верхняя карта</param>
        public static void CheckStack(Card down, Card up)
        {
            if (CanStack(down, up)) return;
            var s = $"невозможно положить карту {up} на {down}";
#if UNITY_EDITOR
            //throw new ArgumentException(s);
            Debug.LogError(s);
#else
            Debug.LogError(s);
#endif
        }

        public static bool IsRed(this Suit suit)
        {
            return suit is Suit.Hearts or Suit.Diamonds;
        }

        public static bool IsBlack(this Suit suit)
        {
            return suit is Suit.Clubs or Suit.Spades;
        }

        public static int ValueNormalized(this Card card) =>
            card.Value % 13;

        public static string ValueString(this Card card) =>
            ((ValueName)card.Value).ToString();

        public static string SuitString(this Suit suit)
        {
            return suit switch
            {
                Suit.Diamonds => "♦",
                Suit.Hearts => "♥",
                Suit.Clubs => "♣",
                Suit.Spades => "♠",
                _ => throw new ArgumentOutOfRangeException(nameof(suit), suit, null)
            };
        }
    }
}