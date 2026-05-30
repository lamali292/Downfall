using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using Downfall.DownfallCode.Artists;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Repurpose : SlimeBossCardModel
{
    public Repurpose() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        this.WithCommand(1);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();
    
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    { 
        if(await SlimeBossCmd.Absorb(ctx, this))
        {
            await SlimeBossCmd.SplitRandom(ctx, Owner, SlimeType.Specialist);
        }
        if (!IsUpgraded) return;
        await SlimeBossCmd.Command(ctx, this);
    }
}