using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class MassRepurpose : SlimeBossCardModel
{
    public MassRepurpose() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCommand(1);
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var absorbed = await SlimeBossCmd.AbsorbAll(ctx, this);
        for (var i = 0; i < absorbed; i++) await SlimeBossCmd.SplitRandom(ctx, Owner, SlimeType.Specialist);
        if (!IsUpgraded) return;
        await SlimeBossCmd.CommandAll(ctx, Owner, this, true);
    }
}