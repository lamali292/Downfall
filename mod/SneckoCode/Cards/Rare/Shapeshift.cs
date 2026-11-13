using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Shapeshift : SneckoCardModel
{
    public Shapeshift() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithTip(DownfallTip.Offclass);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // todo : we generate offclass ancient/rares here. do we want that?
        var rng = Owner.RunState.Rng.CombatCardGeneration;
        var transformations = Owner.Hand
            .Where(c => c.IsTransformable)
            .Select(c => (Card: c, Replacement: SneckoModel.CreateTransformationReplacement(Owner, c, rng)))
            .Where(e => e.Replacement != null)
            .Select(e => new CardTransformation(e.Card, e.Replacement!))
            .ToList();

        var results = await CardCmd.Transform(transformations, rng);
        if (!IsUpgraded) return;
        foreach (var result in results.Where(r => r.success))
            CardCmd.Upgrade(result.cardAdded);
    }
}