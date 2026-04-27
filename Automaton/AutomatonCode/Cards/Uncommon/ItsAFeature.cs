using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class ItsAFeature : AutomatonCardModel
{
    public ItsAFeature() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<ItsAFeaturePower>(3, 1);
        WithTip(DownfallTip.Status);
        WithTip(CardKeyword.Exhaust);
        WithTip(StaticHoverTip.Block);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ItsAFeaturePower>(ctx, this);
    }
}