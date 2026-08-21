using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Cards.Curse;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Enchantment;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Starter relic. At the start of each combat, add a Memento into your hand.
/// </summary>
public sealed class ScorchedLocket : HermitRelicModel
{
    public ScorchedLocket() : base(RelicRarity.Starter)
    {
       WithCardTip<MementoCard>();
       WithTip<Seething>();
    }



    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (Owner.PlayerCombatState is not { TurnNumber: 1 } || player != Owner) return;
        await DownfallCardCmd.GiveCard<MementoCard>(Owner, PileType.Hand, action: EnchantSeething);
        Flash();
    }
    
    private static void EnchantSeething(MementoCard card)
    {
        CardCmd.Enchant<Seething>(card, 1);
    }
}