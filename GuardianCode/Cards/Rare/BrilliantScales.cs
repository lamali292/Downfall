using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class BrilliantScales : GuardianCardModel
{
    public BrilliantScales() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<BrilliantScalesPower>(1, false);
    }


    public override int GemSlots => IsUpgraded ? 3 : 2;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await CommonActions.ApplySelf<BrilliantScalesPower>(ctx, this);
        power?.SetCard(this);
    }
}