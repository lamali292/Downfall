using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class LeechEnergy : SlimeBossCardModel
{
    public LeechEnergy() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5, 3);
        WithEnergy(1);
        WithCards(1);
        WithVar("WeakNeeded", 3);
        WithTip<WeakPower>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var hasEnoughWeak = SlimeBossCmd.HasWeak(this, cardPlay, DynamicVars["WeakNeeded"].IntValue);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!hasEnoughWeak) return;
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await CommonActions.Draw(this, ctx);
    }
}
