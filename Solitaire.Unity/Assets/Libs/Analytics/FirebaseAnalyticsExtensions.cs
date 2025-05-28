using Scripts.Analytics;
using System;

namespace Libs.Analytics
{
    static class FirebaseAnalyticsExtensions
    {
        public static ParameterWrapper ToFirebaseParameter(this GameType gameType)
        {
            switch (gameType) {
                case GameType.OneOnOne:
                    return new ParameterWrapper(FirebaseAnalyticsController.PARAMETER_GAME_TYPE, "OneOnOne");
                case GameType.OneOnThree:
                    return new ParameterWrapper(FirebaseAnalyticsController.PARAMETER_GAME_TYPE, "OneOnThree");
                case GameType.ThreeOnThree:
                    return new ParameterWrapper(FirebaseAnalyticsController.PARAMETER_GAME_TYPE, "ThreeOnThree");
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
            }
        }
    }
}