using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Displays;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public sealed class Refactor() : AutomatonCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("Scry", 4),
        new BlockVar(4, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromKeyword(DownfallKeywords.Scry),
        HoverTipFactory.FromKeyword(DownfallKeywords.Status)
    ];

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var result = await ScryCmd.Execute(ctx, Owner, DynamicVars["Scry"].IntValue);

        var statuses = result.Discarded.Where(c => c.Type == CardType.Status).ToList();
        foreach (var status in statuses)
            await CardCmd.Exhaust(ctx, status);

        if (statuses.Count > 0)
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.IntValue * statuses.Count,
                DynamicVars.Block.Props, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }
}