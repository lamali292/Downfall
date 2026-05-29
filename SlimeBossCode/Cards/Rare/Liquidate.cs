using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using Downfall.DownfallCode.Artists;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Liquidate : SlimeBossCardModel
{
    public Liquidate() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override Artist Artist => Artist.Get<Opal>();

    // TODO: Implement
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}