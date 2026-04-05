using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class AwakenedForm : AwakenedCardModel
{
    public AwakenedForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<CuriosityPower>(1);
        WithPower<RitualPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded)
            await AwakenedCmd.Awaken(Owner, ctx);
        await CommonActions.ApplySelf<CuriosityPower>(this);
        await CommonActions.ApplySelf<RitualPower>(this);
    }
}