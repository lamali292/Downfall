using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class MaxOutput : AutomatonCardModel
{
    public MaxOutput() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithTip(AutomatonTip.Insert);
        WithTip(typeof(Dazed));
        WithPower<MaxOutputPower>(1);
        WithCostUpgradeBy(-1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(ctx, 3, cardPlay.Card.Owner);
        await CommonActions.ApplySelf<MaxOutputPower>(ctx, this);
    }
}