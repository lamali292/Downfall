using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class MassRepurpose : SlimeBossCardModel
{
    public MassRepurpose() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        this.WithCommand(1);
    }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var absorbed = await SlimeBossCmd.AbsorbAll(ctx, this);
        for (var i = 0; i < absorbed; i++) await SlimeBossCmd.SplitRandom(ctx, Owner, SlimeType.Specialist);
        if (!IsUpgraded) return;
        await SlimeBossCmd.CommandAll(ctx, Owner, this, ValueProp.Move);
    }
}