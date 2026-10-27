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

    /// Required so the growth survives a downgrade (the game rebuilds the dynamic vars), same as Rampage.
    private decimal ExtraBlock
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        DynamicVars.Block.BaseValue += DynamicVars["Increase"].BaseValue;
        ExtraBlock += DynamicVars["Increase"].BaseValue;
        EnergyCost.AddThisCombat(DynamicVars.Energy.IntValue);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Block.BaseValue += ExtraBlock;
    }
}