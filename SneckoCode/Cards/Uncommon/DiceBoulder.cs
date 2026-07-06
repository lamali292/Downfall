using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class DiceBoulder : SneckoCardModel
{
    public DiceBoulder() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(7, 1);
        WithVar("Increase", 4, 1);
        WithEnergy(1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        DynamicVars.Block.UpgradeValueBy(DynamicVars["Increase"].BaseValue);
        EnergyCost.UpgradeBy(DynamicVars.Energy.IntValue);
    }
}