using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Displays;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class FeatherWhirl : AwakenedCardModel
{
    public FeatherWhirl() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(typeof(PlumeJab));
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        if (IsUpgraded)
            x += 1;
        await DownfallCardCmd.GiveCards<PlumeJab>(Owner, PileType.Hand, x);
    }
}