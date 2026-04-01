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
public class InfiniteBeams : AutomatonCardModel
{
    public InfiniteBeams() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithTip(new TooltipSource(card =>
        {
            var beam = ModelDb.GetById<MinorBeam>(ModelDb.Card<MinorBeam>().Id).ToMutable();
            if (card.IsUpgraded) beam.UpgradeInternal();
            return HoverTipFactory.FromCard(beam);
        }));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded)
            await PowerCmd.Apply<InfiniteBeamsUpgradedPower>(Owner.Creature, 1, Owner.Creature, this);
        else
            await PowerCmd.Apply<InfiniteBeamsPower>(Owner.Creature, 1, Owner.Creature, this);
    }
}