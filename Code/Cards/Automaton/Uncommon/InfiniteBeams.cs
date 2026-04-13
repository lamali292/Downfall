using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Powers.Automaton;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class InfiniteBeams() : AutomatonCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var card = ModelDb.GetById<MinorBeam>(ModelDb.Card<MinorBeam>().Id).ToMutable();
            if (IsUpgraded) card.UpgradeInternal();
            return [HoverTipFactory.FromCard(card)];
        }
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded)
            await PowerCmd.Apply<InfiniteBeamsUpgradedPower>(Owner.Creature, 1, Owner.Creature, this);
        else
            await PowerCmd.Apply<InfiniteBeamsPower>(Owner.Creature, 1, Owner.Creature, this);
    }
}