using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class TornadoPunch : ChampCardModel
{
    public TornadoPunch() : base(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(12, 2);
        WithBlock(7, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var result = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var a = result.Results.Count(r => r.TotalDamage > 0);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue * a, ValueProp.Move, cardPlay);
    }
}