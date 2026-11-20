using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SoraMod.SoraModCode.Keywords;

public class SoraModKeywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)] public static CardKeyword Link;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)] public static CardKeyword Magic;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)] public static CardKeyword Finisher;
}