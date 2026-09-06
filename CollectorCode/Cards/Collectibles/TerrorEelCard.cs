using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;

public class TerrorEelCard
    : Collectible<TerrorEelElite>
{
    public TerrorEelCard() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.Self, 0.3f)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithPower<VulnerablePower>(3, 2);
    }
    
    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card != this || CombatState == null) return;
        await CommonActions.Apply<VulnerablePower>(ctx, CombatState.HittableEnemies, this);
    }
}