using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Downfall.Code.Cards.Awakened.Multiplayer;

[Pool(typeof(AwakenedCardPool))]
public class DarkIncantation : AwakenedCardModel
{
    public DarkIncantation() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        WithPower<RitualPower>(2);
        WithKeywords(CardKeyword.Exhaust);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var a = cardPlay.Card.Owner.GetRelic<Akabeko>();
        if (cardPlay.Target == null) return;
        await MyCommonActions.Apply<RitualPower>(cardPlay.Target, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<RitualPower>().UpgradeValueBy(1);
    }
}