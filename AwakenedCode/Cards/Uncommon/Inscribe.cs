using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Displays;
using Awakened.AwakenedCode.Extensions;
using Awakened.AwakenedCode.Piles;
using Awakened.AwakenedCode.Vfx;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Inscribe : AwakenedCardModel
{
    public Inscribe() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithConjure(e => e.IsUpgraded);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded)
            await AwakenedCmd.Conjure(Owner);

        var combatState = Owner.Creature.CombatState!;

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

        spellbook.AddPersistentType(chosen);
        spellbook.AddPersistentType(chosen);

        var dupe = chosen.CreateClone();
        var a = await CardPileCmd.Add([chosen, dupe], AwakenedPile.Spellbook);
        CardCmd.PreviewCardPileAdd(a, 0.2f);
        //AwakenedDisplay.RefreshSpellDisplays(Owner);
    }
}