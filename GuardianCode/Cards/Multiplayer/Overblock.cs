using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Multiplayer;

[Pool(typeof(GuardianCardPool))]
public class Overblock : GuardianCardModel
{
    public Overblock() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithTip(GuardianTip.DefensiveMode);
        WithTip(StaticHoverTip.Block);
        this.WithTip<ThornsPower>();
        this.WithPower<OverblockBlockPower>(16, false);
        this.WithPower<OverblockThornsPower>(3, false);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<OverblockBlockPower>(ctx, this);
        await CommonActions.ApplySelf<OverblockThornsPower>(ctx, this);
    }
}