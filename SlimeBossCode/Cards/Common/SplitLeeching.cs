using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class SplitLeeching : SlimeBossCardModel
{
    public SplitLeeching() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        this.WithCommand(2, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await SlimeBossCmd.Split<LeechingSlime>(ctx, Owner);
        await SlimeBossCmd.Command(ctx, this);
    }
}