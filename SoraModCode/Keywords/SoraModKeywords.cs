using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SoraMod.SoraModCode.Character;

public class SoraModKeywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)] public static CardKeyword Friend;
}