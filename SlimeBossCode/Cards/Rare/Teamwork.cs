using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;
using Downfall.DownfallCode.Artists;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Teamwork : SlimeBossCardModel
{
    protected override bool HasEnergyCostX => true;

    public Teamwork() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithCommand(0);
        WithBlock(5, 3);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        await SlimeBossCmd.Command(ctx, Owner, x, this);
        for (var i = 0; i < x; i++)
        {
            await CommonActions.CardBlock(this, cardPlay);
        }
    }
}