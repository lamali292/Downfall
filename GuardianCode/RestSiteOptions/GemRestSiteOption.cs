using BaseLib.Abstracts;
using Downfall.DownfallCode.Nodes;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;

namespace Guardian.GuardianCode.RestSiteOptions;

public class GemRestSiteOption(Player owner) : CustomRestSiteOption(owner)
{
    public const string Id = "DOWNFALL-GEM";

    private CardModel? _gem;
    private CardModel? _gemHolder;

    public override string OptionId => Id;

    public override string CustomIconPath => "rest_site_option_gem.png".RestSitePath<Core.Guardian>();

    public override async Task<bool> OnSelect()
    {
        if (!IsEnabled) return false;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1)
        {
            Cancelable = true,
            RequireManualConfirmation = false
        };

        List<CardModel> cardModel;
        var choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(Owner);
        if (CardSelectCmd.ShouldSelectLocalCard(Owner))
        {
            var gems = Owner.GetDeck(c => c is IGemCard);
            var gemHolder = Owner.GetDeck(c => c is IGemSocketCard { FreeSlots: > 0 });
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
        var hasGems = Owner.GetDeck().Any(c => c is IGemCard);
        var hasSlots = Owner.GetDeck().Any(c => c is IGemSocketCard { FreeSlots: > 0 });
        IsEnabled = hasGems && hasSlots;
        var button = NRestSiteRoom.Instance?.GetButtonForOption(this);
        if (button == null) return false;
        button.Reload();
        button._isUnclickable = !IsEnabled;
        return false;
    }
}