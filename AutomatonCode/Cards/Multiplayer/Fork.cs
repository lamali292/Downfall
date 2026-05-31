using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Multiplayer;

[Pool(typeof(AutomatonCardPool))]
public class Fork : AutomatonCardModel
{
    public Fork() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();


    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.ToAllPlayerHandSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, e => e.EnergyCost.GetResolved() == 1, this))
            .FirstOrDefault();
        if (card == null || cardPlay.Target == null) return;
        var clone = card.CreateClone();
        clone._owner = cardPlay.Target.Player;
        await CardPileCmd.Add(clone, PileType.Hand);
    }
}