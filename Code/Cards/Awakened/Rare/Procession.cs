using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Procession : AwakenedCardModel
{
    public Procession() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = await CommonActions.SelectSingleCard(this, SelectionScreenPrompt, ctx, PileType.Draw);
        if (card == null) return;
        await CardCmd.AutoPlay(ctx, card, null);
        await DownfallCardCmd.GiveCards<Void>(Owner, PileType.Draw,  card.EnergyCost.GetResolved(), CardPilePosition.Random);
    }


    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
   
}