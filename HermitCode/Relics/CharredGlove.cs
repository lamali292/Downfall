using BaseLib.Extensions;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
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
    public CharredGlove() : base(RelicRarity.Rare)
    {
        WithPower<VigorPower>(3);
    }

    protected override async Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? player)
    {
        if (player != Owner || card.Type != CardType.Curse) return;
        Flash();
        await MyCommonActions.ApplySelf<VigorPower>(ctx, this);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner || card.Type != CardType.Curse) return;
        Flash();
        await MyCommonActions.ApplySelf<VigorPower>(ctx, this);
    }
}