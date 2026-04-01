using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Multiplayer;

[Pool(typeof(AwakenedCardPool))]
public class DarkIncantation  : AwakenedCardModel
{
    public DarkIncantation() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        WithPower<RitualPower>(2);
    }
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await MyCommonActions.Apply<RitualPower>(cardPlay.Target, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<RitualPower>().UpgradeValueBy(1);
    }
}