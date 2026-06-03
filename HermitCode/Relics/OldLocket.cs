using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Cards.Curse;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Starter relic. At the start of each combat, add a Memento into your hand.
/// </summary>
public sealed class OldLocket : HermitRelicModel
{
    public OldLocket() : base(RelicRarity.Starter)
    {
        WithTips(e => HoverTipFactory.FromCardWithCardHoverTips<MementoCard>());
    }

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<ClaspedLocket>();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (combatState.RoundNumber > 1 || player != Owner) return;
        await DownfallCardCmd.GiveCard<MementoCard>(Owner, PileType.Hand);
        Flash();
    }
}