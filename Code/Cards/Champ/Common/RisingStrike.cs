using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class RisingStrike : ChampCardModel
{
    public RisingStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Retain);
        WithDamage(8, 3);
        WithTags(CardTag.Strike);
    }

    private bool WasLastCardPlayedFinisher => CombatManager.Instance.History.CardPlaysStarted
        .LastOrDefault(e =>
            e.CardPlay.Card.Owner == Owner &&
            e.CardPlay.Card != this)?
        .CardPlay.Card.Tags.Contains(DownfallTag.Finisher) ?? false;

    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedFinisher;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(WasLastCardPlayedFinisher ? 2 : 1).Execute(ctx);
    }
}