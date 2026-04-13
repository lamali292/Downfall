using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Collector.Token;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Collector.Common;

[Pool(typeof(CollectorCardPool))]
public class BramblesparKindling : CollectorCardModel
{
    public BramblesparKindling() : base(-1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithTip(new TooltipSource(card =>
        {
            var beam = ModelDb.GetById<BurningStrike>(ModelDb.Card<BurningStrike>().Id).ToMutable();
            if (card.IsUpgraded) beam.UpgradeInternal();
            return HoverTipFactory.FromCard(beam);
        }));
        WithKeyword(CardKeyword.Unplayable);
    }
    

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card != this) return;
        await DownfallCardCmd.GiveCard<BurningStrike>(Owner, PileType.Hand, upgraded : IsUpgraded);
    }
}