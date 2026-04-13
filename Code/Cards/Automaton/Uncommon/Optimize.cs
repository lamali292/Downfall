using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Character.Automaton;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Automaton;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Optimize() : AutomatonCardModel(0, CardType.Power, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OptimizePower>(3)
    ];

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Apply<OptimizePower>(Owner.Creature, DynamicVars["OptimizePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["OptimizePower"].UpgradeValueBy(2);
    }
}