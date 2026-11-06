using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Teamwork : SlimeBossCardModel
{
    public Teamwork() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCommand(0);
        WithBlock(5, 3);
    }

    protected override bool HasEnergyCostX => true;

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        await SlimeBossCmd.Command(ctx, Owner, x, true, this);
        for (var i = 0; i < x; i++) await CommonActions.CardBlock(this, cardPlay);
    }
}