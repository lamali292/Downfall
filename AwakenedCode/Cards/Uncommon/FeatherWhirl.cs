using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class FeatherWhirl : AwakenedCardModel
{
    public FeatherWhirl() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithTip<PlumeJab>();
    }
    
    protected override Artist Artist => Artist.Get<Opal>();

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        if (IsUpgraded)
            x += 1;
        await DownfallCardCmd.GiveCards<PlumeJab>(Owner, PileType.Hand, x);
    }
}