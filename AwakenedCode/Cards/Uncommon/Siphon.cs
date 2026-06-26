using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Siphon : AwakenedCardModel, IChantable
{
    public Siphon() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 2);
        this.WithTip<StrengthPower>();
        this.WithPower<SiphonPower>(2, false);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public bool HasChanted { get; set; } = false;

    public async Task PlayChantEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await CommonActions.ApplySelf<SiphonPower>(ctx, this);
        await CommonActions.Apply<SiphonPower>(ctx, cardPlay.Target, this, -DynamicVars.Power<SiphonPower>().BaseValue);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}

public class SiphonPower : CustomTemporaryPowerModelWrapper<Siphon, StrengthPower>;