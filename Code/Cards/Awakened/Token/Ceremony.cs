using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Ceremony : AwakenedCardModel
{
    public Ceremony() : base(0, CardType.Power, CardRarity.Token, TargetType.None)
    {
        WithPower<StrengthPower>(1, 1);
        WithKeywords(CardKeyword.Retain);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(this, DynamicVars.Power<StrengthPower>().BaseValue);
    }

    // Fervent Worship stuff
    public override Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
    {
        if (card != this) return Task.CompletedTask;
        var a = Owner.Creature.GetPowerAmount<FerventWorshipPower>();
        if (a == 0) return Task.CompletedTask;
        EnergyCost.UpgradeBy(a);
        BaseReplayCount += a;
        return Task.CompletedTask;
    }


    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is not FerventWorshipPower || power.Owner != Owner.Creature) return Task.CompletedTask;
        var i = (int)amount;
        EnergyCost.UpgradeBy(i);
        BaseReplayCount += i;
        return Task.CompletedTask;
    }
}