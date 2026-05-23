using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class TornadoPunch : ChampCardModel, IDefensiveComboCard
{
    private int _lastHitCount;

    public TornadoPunch() : base(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(12, 2);
        WithBlock(7, 2);
        WithVar("LastHitCount", 0);
    }

    public async Task DefensiveComboEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        for (int i = 0; i < _lastHitCount; i++)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block,
                cardPlay);
        }
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var result = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        _lastHitCount = result.Results.SelectMany(r => r).Count(x => x.TotalDamage > 0);
    }
}