using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Murder : AwakenedCardModel
{
    public Murder() : base(1, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
    {
        WithDamage(4);
        this.WithRepeat(4);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override Artist Artist => Artist.Get<Eudaimonia>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(DynamicVars.Repeat.IntValue).Execute(ctx);
    }
}