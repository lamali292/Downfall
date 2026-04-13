using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.Awakened.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Displays;
using Downfall.Code.Piles;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Inscribe : AwakenedCardModel
{
    public Inscribe() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
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

        var spellbook = AwakenedCmd.GetSpellbook(Owner);
        if (spellbook == null) return;
        
            spellbook.AddPersistentType(chosen.GetType());
            spellbook.AddPersistentType(chosen.GetType());
            
            var dupe = chosen.CreateClone();
            spellbook.AddInternal(chosen);
            spellbook.AddInternal(dupe);
            
            AwakenedDisplay.Refresh(Owner);
        
    }
 
}