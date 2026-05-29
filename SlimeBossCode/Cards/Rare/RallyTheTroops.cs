using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using Downfall.DownfallCode.Artists;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class RallyTheTroops : SlimeBossCardModel
{
    public RallyTheTroops() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override Artist Artist => Artist.Get<Opal>();

    // TODO: Implement
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}