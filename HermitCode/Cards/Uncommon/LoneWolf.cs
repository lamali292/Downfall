using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class LoneWolf : HermitCardModel
{
    public LoneWolf() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var handPile = PileType.Hand.GetPile(Owner);
        if (handPile.Cards.Count == 0) return;
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var chosen = (await CardSelectCmd.FromHand(ctx, Owner, prefs, null, this)).FirstOrDefault();
        if (chosen == null) return;
        chosen.SetToFreeThisTurn();
        var toDiscard = handPile.Cards.Where(c => c != chosen).ToList();
        await CardPileCmd.Add(toDiscard, PileType.Discard);
    }
}