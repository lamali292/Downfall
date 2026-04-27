using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class ClobberStrike : ChampCardModel
{
    public ClobberStrike() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithBlock(1);
        WithDamage(6, 3);
        WithTip(StaticHoverTip.Block);
        WithTags(CardTag.Strike);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var attackCommand = await CommonActions.CardAttack(this, cardPlay.Target).Execute(ctx);
        var unblockedDamage = attackCommand.Results.Sum(r => r.UnblockedDamage);
        await CreatureCmd.GainBlock(Owner.Creature, unblockedDamage, ValueProp.Move, cardPlay);
    }
}