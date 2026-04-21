using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class DarkwillowKindling : CollectorCardModel
{
    public DarkwillowKindling() : base(-1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithKeyword(CardKeyword.Unplayable);
        WithPower<ReserveNextTurnPower>(2);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        await CommonActions.ApplySelf<ReserveNextTurnPower>(this);
    }
}