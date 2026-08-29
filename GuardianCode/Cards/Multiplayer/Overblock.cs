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
    public Overblock() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithCostUpgradeBy(-1);
        WithTip(GuardianTip.DefensiveMode);
        WithTip(StaticHoverTip.Block);
        WithTip<ThornsPower>();
        WithPower<OverblockBlockPower>(12, false);
        WithVar("OverblockThornsPower", 3);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        (await CommonActions.ApplySelf<OverblockBlockPower>(ctx, this))?.IncrementThorns(
            DynamicVars["OverblockThornsPower"].BaseValue);
    }
}