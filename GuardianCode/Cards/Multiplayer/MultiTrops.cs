using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Multiplayer;

[Pool(typeof(GuardianCardPool))]
public class MultiTrops : GuardianCardModel
{
    public MultiTrops() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithPower<ThornsPower>(2, 1);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<ThornsPower>(ctx, this, cardPlay);
    }
}