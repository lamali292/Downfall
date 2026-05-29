using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using Downfall.DownfallCode.Artists;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class NibbleAndLick : SlimeBossCardModel
{
    public NibbleAndLick() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    // TODO: Implement
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}