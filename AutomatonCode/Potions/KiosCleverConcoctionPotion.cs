using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Potions;

[Pool(typeof(AutomatonPotionPool))]
public class KiosCleverConcoctionPotion : AutomatonPotionModel
{
    public KiosCleverConcoctionPotion() : base(PotionRarity.Rare, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithTip(AutomatonTip.Encode);
    }

    protected override Artist Artist => Artist.Get<Chimedragon>();
    
    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        var rng = Owner.RunState.Rng.CombatCardSelection;

        var cards =  CardFactory.FilterForCombat(Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => AutomatonCmd.IsEncodable(c) && c.Rarity != CardRarity.Token)).ToList();
        
        FunctionCard? functionCard = null;
        while (functionCard == null)
        {
            var choices = CardFactory.GetDistinctForCombat(Owner, cards, 3, rng).ToList();
            var selected = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
            if (selected == null) break;
            functionCard = await AutomatonCmd.EncodeCard(selected, ctx);
        }
    }
}