using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Extensions;
using BaseLib.Utils;
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

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        var rng = Owner.RunState.Rng.CombatCardSelection;

        var cards = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => AutomatonCmd.IsEncodable(c) && c.Rarity != CardRarity.Token).ToList();

        var max = AutomatonCmd.GetMax(Owner);
        while (Owner.GetEncode().Count < max)
        {
            var countBefore = Owner.GetEncode().Count;
            var choices = CardFactory.GetDistinctForCombat(Owner, cards, 3, rng).ToList();
            var selected = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
            if (selected == null) break;
            await AutomatonCmd.EncodeCard(selected, ctx);
            if (Owner.GetEncode().Count < countBefore + 1)
                return;
        }
    }
}