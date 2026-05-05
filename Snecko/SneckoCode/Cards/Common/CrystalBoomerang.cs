using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class CrystalBoomerang : SneckoCardModel
{
    public CrystalBoomerang() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(5, 3);
        WithCards(1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var result = (await DownfallCardCmd.SelectCardToMovePiles(ctx, this, PileType.Discard, PileType.Hand)).ToList().FirstOrDefault();
        if (!result.success) return;
        if (!SneckoCmd.IsOffclass(this, result.cardAdded)) return;
        await CommonActions.CardBlock(this, cardPlay);
    }
}