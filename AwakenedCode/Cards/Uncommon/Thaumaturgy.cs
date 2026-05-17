using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Thaumaturgy : AwakenedCardModel
{
    public Thaumaturgy() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<DexterityPower>(1, 1);
        WithPower<ThaumaturgyPower>(2, false);
        WithTip(typeof(Ceremony));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DexterityPower>(ctx, this, DynamicVars.Dexterity.BaseValue);
        await CommonActions.ApplySelf<ThaumaturgyPower>(ctx, this);
    }
}