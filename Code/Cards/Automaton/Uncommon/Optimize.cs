using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Automaton;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Optimize : AutomatonCardModel
{
    public Optimize() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithTip(DownfallKeyword.Encode);
        WithPower<OptimizePower>(3, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Apply<OptimizePower>(Owner.Creature, DynamicVars.Power<OptimizePower>().BaseValue,
            Owner.Creature,
            this);
    }
}