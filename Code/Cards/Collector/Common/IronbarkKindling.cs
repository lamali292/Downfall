using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Collector.Common;

[Pool(typeof(CollectorCardPool))]
public class IronbarkKindling : CollectorCardModel
{
    public IronbarkKindling() : base(-1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(CardKeyword.Unplayable);
        WithBlock(9);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
    }
}