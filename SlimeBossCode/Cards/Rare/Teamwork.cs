using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Teamwork : SlimeBossCardModel
{
    protected override bool HasEnergyCostX => true;

    public Teamwork() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithCommand(1);
        WithBlock(5, 3);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        await SlimeBossCmd.Command(ctx, Owner, x);
        for (var i = 0; i < x; i++)
        {
            await CommonActions.CardBlock(this, cardPlay);
        }
    }
}