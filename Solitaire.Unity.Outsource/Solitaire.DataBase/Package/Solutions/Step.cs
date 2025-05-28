using System;

namespace Solitaire.DataBase.Solutions
{
    /// <summary>
    /// описывает один шаг в базе данных
    /// </summary>
    public struct Step: IEquatable<Step>
    {
        /// <summary>
        /// тип хода
        /// </summary>
        public StepTypes StepType { get; set; }
        /// <summary>
        /// аргумент 0, для ходов, где есть только 1 аргумент
        /// <para><see cref="StepTypes.MoveCards"/> srcTableauIdx=Index of the source tableau</para>
        /// </summary>
        public int Arg0 { get; set; }
        /// <summary>
        /// <para><see cref="StepTypes.MoveCards"/> depth=How many cards to move in the source tableau</para>
        /// </summary>
        public int Arg1 { get; set; }
        /// <summary>
        /// <para><see cref="StepTypes.MoveCards"/> destTableauIdx=Index of the destination tableau</para>
        /// </summary>
        public int Arg2 { get; set; }

        public Step(StepTypes stepType)
        {
            StepType = stepType;
            Arg0 = 0;
            Arg1 = 0;
            Arg2 = 0;
        }
        public Step(StepTypes stepType, int arg0)
        {
            StepType = stepType;
            Arg0 = arg0;
            Arg1 = 0;
            Arg2 = 0;
        }
        public Step(StepTypes stepType, int arg0, int arg1)
        {
            StepType = stepType;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = 0;
        }
        public Step(StepTypes stepType, int arg0, int arg1, int arg2)
        {
            StepType = stepType;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public static Step MoveCards(int srcTableauIdx, int depth, int destTableauIdx)
        {
            return new Step(StepTypes.MoveCards, srcTableauIdx, depth, destTableauIdx);
        }
        public static Step MoveWholeTableau(int srcTableauIdx, int destTableauIdx)
        {
            return new Step(StepTypes.MoveWholeTableau, srcTableauIdx, destTableauIdx);
        }
        public static Step MoveToFoundation(int srcTableauIdx, int destFoundationIdx)
        {
            return new Step(StepTypes.MoveToFoundation, srcTableauIdx, destFoundationIdx);
        }
        public static Step MoveFromWaste(int destTableauIdx)
        {
            return new Step(StepTypes.MoveFromWaste, destTableauIdx);
        }
        public static Step MoveFromWasteToFoundation(int destFoundationIdx)
        {
            return new Step(StepTypes.MoveFromWasteToFoundation, destFoundationIdx);
        }
        public static Step MoveFromFoundation(int srcFoundationIdx, int destTableauIdx)
        {
            return new Step(StepTypes.MoveFromFoundation, srcFoundationIdx, destTableauIdx);
        }
        public static Step DrawFromStock()
        {
            return new Step(StepTypes.DrawFromStock);
        }

        public override string ToString()
        {
            switch (StepType) {
                case StepTypes.MoveCards:
                    return $"MoveCards: {Arg0 + 1}, {Arg2 + 1}, {Arg1}";
                case StepTypes.MoveToFoundation:
                    return $"MoveToFoundation: {Arg0 + 1}";
                case StepTypes.MoveFromWaste:
                    return $"MoveFromWaste: {Arg0 + 1}";
                case StepTypes.MoveFromWasteToFoundation:
                    return $"MoveFromWasteToFoundation";
                case StepTypes.DrawFromStock:
                    return $"DrawFromStock";
            }

            return $"{StepType}: {Arg0}, {Arg1}, {Arg2}";
        }
        public bool Equals(Step other)
        {
            return StepType == other.StepType && Arg0 == other.Arg0 && Arg1 == other.Arg1 && Arg2 == other.Arg2;
        }
        public override bool Equals(object obj)
        {
            return obj is Step other && Equals(other);
        }
        public override int GetHashCode()
        {
            unchecked {
                var hashCode = (int)StepType;
                hashCode = (hashCode * 397) ^ Arg0;
                hashCode = (hashCode * 397) ^ Arg1;
                hashCode = (hashCode * 397) ^ Arg2;
                return hashCode;
            }
        }
    }

}