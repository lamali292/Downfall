using Downfall.DownfallCode.Extensions;
using Downfall.DownfallCode.Nodes;
using Downfall.DownfallCode.Utils;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;

namespace Guardian.GuardianCode.RestSiteOptions;

public class GemRestSiteOption(Player owner) : CustomRestSiteOption(owner)
{
    private CardModel? _gem;
    private CardModel? _gemHolder;

    public override string OptionId => "DOWNFALL-GEM";

    public override string CustomIconPath => "rest_site_option_gem.png".RestSitePath();

    public override async Task<bool> OnSelect()
    {
        if (!IsEnabled) return false;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1)
        {
            Cancelable = true,
            RequireManualConfirmation = false
        };
        var gems = PileType.Deck.GetPile(Owner).Cards.Where(c => c is IGemCard).ToList();
        var gemHolder = PileType.Deck.GetPile(Owner).Cards.Where(c => c is GuardianCardModel { FreeSlots: > 0 })
            .ToList();
        List<CardModel> cardModel;

        var choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(Owner);
        if (CardSelectCmd.ShouldSelectLocalCard(Owner))
        {
            var a = NGemUpgradeSelectScreen.Create(gems, gemHolder, prefs);
            if (NOverlayStack.Instance == null) return false;
            NOverlayStack.Instance.Push(a);
            cardModel = (await a.CompletionSource.Task).ToList();
            NOverlayStack.Instance.Remove(a);

            RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(Owner, choiceId,
                PlayerChoiceResult.FromMutableDeckCards(cardModel));
        }
        else
        {
            cardModel = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(Owner, choiceId))
                .AsDeckCards().ToList();
        }

        CardSelectCmd.LogChoice(Owner, cardModel);
        if (cardModel.Count != 2) return false;
        _gem = cardModel.First();
        _gemHolder = cardModel.Last();
        if (_gem == null || _gemHolder == null)
            return false;
        await GuardianCmd.PutGemIn(_gem, _gemHolder);
        var hasGems = PileType.Deck.GetPile(Owner).Cards.Any(c => c is IGemCard);
        var hasSlots = PileType.Deck.GetPile(Owner).Cards.Any(c => c is GuardianCardModel { FreeSlots: > 0 });
        IsEnabled = hasGems && hasSlots;
        var button = NRestSiteRoom.Instance?.GetButtonForOption(this);
        if (button == null) return false;
        button.Reload();
        button._isUnclickable = !IsEnabled;
        return false;
    }
}