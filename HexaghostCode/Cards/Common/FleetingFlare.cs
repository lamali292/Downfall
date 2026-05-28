using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class FleetingFlare : HexaghostCardModel
{
    public FleetingFlare() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await HexaghostCmd.Ignite(ctx, Owner);
        await HexaghostCmd.Extinguish(Owner);
    }
}