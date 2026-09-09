using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;
namespace Collector.CollectorCode.Cards.Collectibles;

public class DecimillipedeCard : Collectible<DecimillipedeElite>
{
    public DecimillipedeCard() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0.66f)
    {
        WithKindle(4, 2);
        WithBlock(4, 2);
        WithPower<BlockNextTurnPower>(4, 2, false);
        WithKeyword(CardKeyword.Exhaust);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card != this) return;
        await CommonActions.ApplySelf<BlockNextTurnPower>(ctx, this);
        await CollectorCmd.Kindle(ctx, this);
        await DownfallCreatureCmd.GainBlock(Owner.Creature, this);
    }
}
