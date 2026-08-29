using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Whenever you draw a Curse, your next attack deals 3 more damage.
/// </summary>
public sealed class CharredGlove : HermitRelicModel
{
    public CharredGlove() : base(RelicRarity.Uncommon)
    {
        WithPower<VigorPower>(3);
    }

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? player)
    {
        if (player != Owner || card.Type != CardType.Curse) return;
        Flash();
        await MyCommonActions.ApplySelf<VigorPower>(new BlockingPlayerChoiceContext(), this);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner || card.Type != CardType.Curse) return;
        Flash();
        await MyCommonActions.ApplySelf<VigorPower>(ctx, this);
    }
}