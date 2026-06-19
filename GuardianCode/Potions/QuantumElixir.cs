using Guardian.GuardianCode.Core;
using BaseLib.Utils;
using Guardian.GuardianCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Potions;

[Pool(typeof(GuardianPotionPool))]
public class QuantumElixir : GuardianPotionModel
{
    public QuantumElixir() : base(PotionRarity.Rare, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithCards(3);
        WithTip(GuardianTip.Stasis);
    }

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        var rng = Owner.RunState.Rng.CombatCardSelection;

        var cards = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.Rarity != CardRarity.Token).ToList();
        
        while (GuardianCmd.CanPutIntoStasis(Owner, silent: true))
        {
            var countBefore = GuardianCmd.GetStasisCount(Owner);

            var choices = CardFactory.GetDistinctForCombat(Owner, cards, DynamicVars.Cards.IntValue, rng).ToList();
            var selected = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
            if (selected == null) break;

            await GuardianCmd.PutIntoStasis(selected, ctx, this);
            if (GuardianCmd.GetStasisCount(Owner) < countBefore + 1)
                return;
        }
    }
}