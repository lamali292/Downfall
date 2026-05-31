using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Liquidate : SlimeBossCardModel
{
    public Liquidate() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<StrengthPower>(2);
        WithPower<PotencyPower>(2, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this, -DynamicVars.Strength.BaseValue);
        await CommonActions.ApplySelf<PotencyPower>(ctx, this);
    }
}