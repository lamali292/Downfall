using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Automaton;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class ItsAFeature : AutomatonCardModel
{
    public ItsAFeature() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<ItsAFeaturePower>(3);
        WithTip(DownfallKeyword.Status);
        WithTip(CardKeyword.Exhaust);
        WithTip(StaticHoverTip.Block);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ItsAFeaturePower>(Owner.Creature, DynamicVars.Power<ItsAFeaturePower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<ItsAFeaturePower>().UpgradeValueBy(1);
    }
}