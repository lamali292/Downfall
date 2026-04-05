using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class SpreadingSpores : AwakenedCardModel
{
    public SpreadingSpores() : base(0, CardType.Power, CardRarity.Token, TargetType.None)
    {
        WithKeywords(CardKeyword.Ethereal);
        WithPower<ThornsPower>(2, 2);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ThornsPower>(this);
        var card = CreateClone();
        var result = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, true, CardPilePosition.Random);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result, 0.1f, CardPreviewStyle.MessyLayout);
    }
}