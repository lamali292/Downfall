using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
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
        WithKindle(5, 2);
        WithBlock(5, 2);
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card != this) return;
     
        await CollectorCmd.Kindle(ctx, this);
        var block = await DownfallCreatureCmd.GainBlock(Owner.Creature, this);
        await CommonActions.ApplySelf<BlockNextTurnPower>(ctx, this, block);
    }
}
