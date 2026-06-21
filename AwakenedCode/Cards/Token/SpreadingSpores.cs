using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class SpreadingSpores : AwakenedCardModel
{
    public SpreadingSpores() : base(0, CardType.Power, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Ethereal);
        WithPower<ThornsPower>(2, 2);
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ThornsPower>(ctx, this);
        var card = CreateClone();
        var result = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, Owner, CardPilePosition.Random);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result, 0.1f, CardPreviewStyle.MessyLayout);
    }
}