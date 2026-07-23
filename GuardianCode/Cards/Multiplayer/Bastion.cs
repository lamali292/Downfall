using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Guardian.GuardianCode.Cards.Multiplayer;

[Pool(typeof(GuardianCardPool))]
public class Bastion : GuardianCardModel
{
    public Bastion() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<BastionPower>(1, 1, false);
        WithTip(StaticHoverTip.Block);
        WithTip(GuardianTip.Brace);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<BastionPower>(ctx, this);
    }
    
}