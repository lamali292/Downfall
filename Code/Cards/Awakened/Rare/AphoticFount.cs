using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using Downfall.Code.Powers.Downfall;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class AphoticFount : AwakenedCardModel
{
    public AphoticFount() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<AphoticFountPower>(1, 1);
        WithTip(typeof(PlatedArmorPower));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await AwakenedCmd.Conjure(Owner, CombatState);
        await MyCommonActions.ApplySelf<AphoticFountPower>(this);
    }
}