using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Teamwork : SlimeBossCardModel
{
    public Teamwork() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(5, 3);
        WithSlimeTip<BruiserSlime>();
        WithTip(SlimeBossTip.Command);
    }

    protected override bool HasEnergyCostX => true;

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        for (var i = 0; i < x; i++)
        {
            await CommonActions.CardBlock(this, cardPlay);
        }
        await SlimeBossCmd.Command<BruiserSlime>(ctx, Owner, x, this);
    }
}
