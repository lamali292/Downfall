using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class SplitWide : AwakenedCardModel
{
    public SplitWide() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        this.WithPower<SplitWidePower>(1, 1, false);
        this.WithTip<StrengthPower>();
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<SplitWidePower>(ctx, cardPlay.Target, this);
    }
}