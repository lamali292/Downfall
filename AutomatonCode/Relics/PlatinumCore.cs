using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Events;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class PlatinumCore : AutomatonRelicModel, IModifyCompiledFunction
{
    public PlatinumCore() : base(RelicRarity.Starter)
    {
        WithCardTip<CoreStrike>();
        WithCardTip<CoreDefend>();
        WithTip(AutomatonKeyword.Encode);
    }

       
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        Flash();
        await Cmd.Wait(0.2f);
        await AutomatonCmd.EncodeCard<CoreStrike>(Owner, ctx);
        await AutomatonCmd.EncodeCard<CoreDefend>(Owner, ctx);
    }

    
    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        var transformations = PileType.Deck.GetPile(Owner).Cards
            .Where(c => c is { IsBasicStrikeOrDefend: true, IsTransformable: true })
            .Select(c => new CardTransformation(c, CreateReplacement(c)))
            .ToList();
        if (transformations.Count > 0)
            await CardCmd.Transform(transformations, null, CardPreviewStyle.MessyLayout);
        Flash();
    }

    private CardModel CreateReplacement(CardModel source)
    {
        var replacement = source.CardScope!.CreateCard(
            source.Tags.Contains(CardTag.Strike) ? ModelDb.Card<CoreStrike>() : ModelDb.Card<CoreDefend>(),
            Owner);
        CopyUpgradeAndEnchant(source, replacement);
        return replacement;
    }
    
    private static void CopyUpgradeAndEnchant(CardModel source, CardModel card)
    {
        if (source.IsUpgraded)
            card.UpgradeInternal();
        var enchant = (EnchantmentModel?)source.Enchantment?.MutableClone();
        if (enchant == null) return;
        if (enchant.CanEnchant(card))
            card.EnchantInternal(enchant, enchant.Amount);
    }
    
    public bool ModifyCompiledFunction(FunctionCard function, Player player)
    {
        if (function.SourceCards.Count(e => e.Tags.Contains(AutomatonTag.Core)) < 2) return false;
        function.EnergyCost.SetUntilPlayed(0);
        return true;
    }

    public Task AfterModifyCompiledFunction(FunctionCard result, Player player)
    {
        Flash();
        return Task.CompletedTask;
    }
}