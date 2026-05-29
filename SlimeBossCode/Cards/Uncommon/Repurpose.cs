using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using Downfall.DownfallCode.Artists;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Repurpose : SlimeBossCardModel
{
    public Repurpose() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    // TODO: Implement
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}