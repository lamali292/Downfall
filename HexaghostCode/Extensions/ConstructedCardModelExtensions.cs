using System.Reflection;
using BaseLib.Abstracts;
using HarmonyLib;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Hexaghost.HexaghostCode.Extensions;

public static class ConstructedCardModelExtensions
{
    public static ConstructedCardModel WithAfterlife(this ConstructedCardModel card)
    {
        card.WithKeywords(CardKeyword.Ethereal, HexaghostKeyword.Afterlife);
        return card;
    }
}