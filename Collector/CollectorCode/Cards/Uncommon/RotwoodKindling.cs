using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class RotwoodKindling : CollectorCardModel
{
    public RotwoodKindling() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithKeyword(CardKeyword.Unplayable);
        WithPower<VulnerablePower>(2, 1);
        WithPower<CollectorDoomPower>(4, 2);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this || CombatState == null) return;
        await CommonActions.Apply<VulnerablePower>(ctx, CombatState.Enemies, this);
        await CommonActions.Apply<CollectorDoomPower>(ctx, CombatState.Enemies, this);
    }
}