using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Rare;

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
        await CommonActions.ApplySelf<CuriosityPower>(ctx, this);
        await CommonActions.ApplySelf<RitualPower>(ctx, this);
    }
}