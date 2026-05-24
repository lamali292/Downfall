using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Extensions.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class DarknessFalls : AwakenedCardModel
{
    public DarknessFalls() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(AwakenedTip.Drained.WithVars(new EnergyVar(1)));
        WithTip(new TooltipSource(_ => HoverTipFactory.Static(StaticHoverTip.Block)));
        WithTip(typeof(StrengthPower));
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DarknessFallsPower>(ctx, this, 4);
        await CommonActions.ApplySelf<DarkblessedPower>(ctx, this, 1);
    }
}