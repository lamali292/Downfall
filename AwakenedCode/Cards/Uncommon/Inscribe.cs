using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Displays;
using Awakened.AwakenedCode.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Inscribe : AwakenedCardModel
{
    public Inscribe() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithConjure(e => e.IsUpgraded);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        if (IsUpgraded)
            await AwakenedCmd.Conjure(Owner, CombatState);

        var combatState = Owner.Creature.CombatState!;

        // Wir erstellen die Auswahlmöglichkeiten
        var choices = new List<CardModel>
        {
            combatState.CreateCard<BurningStudy>(Owner),
            combatState.CreateCard<Cryostasis>(Owner),
            combatState.CreateCard<Darkleech>(Owner),
            combatState.CreateCard<Thunderbolt>(Owner)
        };

        var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
        if (chosen == null) return;

        var spellbook = AwakenedModel.GetOrInitSpellbook(Owner);

        spellbook.AddPersistentType(chosen);
        spellbook.AddPersistentType(chosen);

        var dupe = chosen.CreateClone();
        spellbook.AddInternal(chosen);
        spellbook.AddInternal(dupe);

        AwakenedDisplay.Refresh(Owner);
    }
}