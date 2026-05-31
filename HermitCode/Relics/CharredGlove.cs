using BaseLib.Extensions;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
    public CharredGlove() : base(RelicRarity.Common)
    {
        WithPower<VigorPower>(3);
        WithTip<VigorPower>();
    }


    public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner.Creature) return;
        if (card.Type == CardType.Curse)
        {
            Flash();
            await PowerCmd.Apply<VigorPower>(ctx, Owner!.Creature, DynamicVars.Power<VigorPower>().BaseValue,
                Owner.Creature,
                null);
        }
    }
}