using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class CleanUp : AutomatonCardModel
{
    public CleanUp() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithTip(CardKeyword.Exhaust);
        WithCards(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
      
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs,
            null, this)).FirstOrDefault();
        if (card == null) return;
        await CardCmd.Exhaust(ctx, card);
        var hitCount = card is { Type: CardType.Curse or CardType.Status } ? 2 : 1;
        await CommonActions.CardAttack(this, cardPlay, hitCount).Execute(ctx);
    }
}