using Automaton.AutomatonCode.Cards.Basic;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class PlatinumCore : AutomatonRelicModel
{
    public PlatinumCore() : base(RelicRarity.Starter)
    {
        WithTip(typeof(StrikeAutomaton));
        WithTip(typeof(DefendAutomaton));
        WithTip(AutomatonTip.Encode);
    }
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        Flash();
        if (player != Owner) return;
        if (combatState.RoundNumber == 1)
        {

            var card1 = player.Creature.CombatState!.CreateCard(ModelDb.Card<StrikeAutomaton>(), player);
            var card2 = player.Creature.CombatState!.CreateCard(ModelDb.Card<DefendAutomaton>(), player);
            await AutomatonCmd.EncodeCard(card1, ctx);
            await AutomatonCmd.EncodeCard(card2, ctx);
        }
       
        var cards = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => AutomatonCmd.IsEncodable(c) && c.Rarity != CardRarity.Token).ToList();
        var rng = Owner.RunState.Rng.CombatCardSelection;
        var choice = CardFactory.GetDistinctForCombat(Owner, cards, 1, rng).FirstOrDefault();
        if (choice == null) return;
        await AutomatonCmd.EncodeCard(choice, ctx);
        
    }
}