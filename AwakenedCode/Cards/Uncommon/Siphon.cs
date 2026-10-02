using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Siphon : AwakenedCardModel
{
    public Siphon() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 2);
        WithTip<StrengthPower>();
        WithPower<SiphonPower>(2, 1, false);
        WithKeyword(AwakenedKeyword.Chant);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await ChantCmd.Chant(cardPlay, async () =>
        {
            if (cardPlay.Target == null) return;
            await CommonActions.ApplySelf<SiphonPower>(ctx, this);
            await CommonActions.Apply<SiphonPower>(ctx, cardPlay.Target, this, -DynamicVars.Power<SiphonPower>().BaseValue);
        });
    }
}

public class SiphonPower : CustomTemporaryPowerModelWrapper<Siphon, StrengthPower>;