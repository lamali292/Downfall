using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Collector.Token;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Rare;

[Pool(typeof(CollectorCardPool))]
public class Darkstorm : CollectorCardModel
{
    public Darkstorm() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCards(2, 2);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(typeof(Blightning));
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCard<Blightning>(Owner, PileType.Hand);
        await DownfallCardCmd.GiveCards<Blightning>(Owner, PileType.Draw, DynamicVars.Cards.IntValue, CardPilePosition.Random);
    }
}