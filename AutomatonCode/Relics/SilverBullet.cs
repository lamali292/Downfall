using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class SilverBullet : AutomatonRelicModel
{
    public SilverBullet() : base(RelicRarity.Common)
    {
        WithDamage(2);
    }


    protected override async Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? creator)
    {
        if (creator == null || creator != Owner || card.Type != CardType.Status) return;
        Flash();
        await MyCommonActions.Attack(this, null, TargetType.AllEnemies).Execute(ctx);
    }
}