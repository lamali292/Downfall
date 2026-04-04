using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Thaumaturgy : AwakenedCardModel
{
    public Thaumaturgy() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<DexterityPower>(1, 1);
        WithPower<ThaumaturgyPower>(2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DexterityPower>(this, DynamicVars.Dexterity.BaseValue);
        await CommonActions.ApplySelf<ThaumaturgyPower>(this, DynamicVars.Power<ThaumaturgyPower>().BaseValue);
    }
}