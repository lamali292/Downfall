using System.Runtime.InteropServices;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

namespace Downfall.DownfallCode.Nodes;

[GlobalClass]
public partial class NGemUpgradeSelectScreen :
	Control,
	IOverlayScreen,
	ICardSelector
{
	private readonly HashSet<CardModel> _selectedCards = [];
	public readonly TaskCompletionSource<IEnumerable<CardModel>> CompletionSource = new();
	private IReadOnlyList<CardModel>? _cards;

	// UI Elements
	private NBackButton? _closeButton;
	private NConfirmButton? _confirmButton;

	private IReadOnlyList<CardModel>? _gems;

	// Grid stuff
	private NCardGrid? _grid;
	private MegaRichTextLabel? _infoLabel;

	// State Tracking
	private bool _isSelectingGem = true;
	private NPeekButton? _peekButton;
	private CardSelectorPrefs _prefs;
	private CardModel? _selectedGem;

	private static string ScenePath => "res://Downfall/scenes/screens/gem_upgrade_select_screen.tscn";

	public static IEnumerable<string> AssetPaths => [ScenePath];
	private IEnumerable<Control> PeekButtonTargets => [_closeButton!, _confirmButton!];

	// Grid stuff
	public async Task<IEnumerable<CardModel>> CardsSelected()
	{
		return await CompletionSource.Task;
	}

	public NetScreenType ScreenType => NetScreenType.CardSelection;

	public Control? DefaultFocusedControl => _grid?.DefaultFocusedControl;
	public Control? FocusedControlFromTopBar => _grid?.FocusedControlFromTopBar;

	public void AfterOverlayShown()
	{
		if (!_prefs.Cancelable || _closeButton == null)
			return;
		_closeButton.Enable();
	}

	public void AfterOverlayHidden()
	{
		_closeButton?.Disable();
	}

	public virtual void AfterOverlayOpened()
	{
	}

	public virtual void AfterOverlayClosed()
	{
		if (_peekButton == null) return;
		_peekButton.SetPeeking(false);
		this.QueueFreeSafely();
	}

	public bool UseSharedBackstop => true;

	public override void _Ready()
	{
		_closeButton = GetNode<NBackButton>((NodePath)"%Close");
		_confirmButton = GetNode<NConfirmButton>((NodePath)"%Confirm");
		_infoLabel = GetNode<MegaRichTextLabel>((NodePath)"%BottomLabel");

		_ = (long)_closeButton.Connect(NClickableControl.SignalName.Released,
			Callable.From(new Action<NButton>(CloseSelection)));
		_ = (long)_confirmButton.Connect(NClickableControl.SignalName.Released,
			Callable.From(new Action<NButton>(ConfirmSelection)));

		if (_prefs.Cancelable)
			_closeButton.Enable();
		else
			_closeButton.Disable();

		ConnectSignalsAndInitGrid();
		RefreshConfirmButtonVisibility();

		// Set initial prompt text
		_infoLabel.Text = _prefs.Prompt.GetFormattedText();
	}

	private void ConnectSignalsAndInitGrid()
	{
		if (_gems == null) return;
		_grid = GetNode<NCardGrid>((NodePath)"%CardGrid");
		_peekButton = GetNode<NPeekButton>((NodePath)"%PeekButton");

		// Start phase 1: Show Gems
		RefreshGrid(_gems);

		_ = (long)_grid.Connect(NCardGrid.SignalName.HolderPressed,
			Callable.From((Action<NCardHolder>)(h => OnCardClicked(h.CardModel!))));
		_ = (long)_grid.Connect(NCardGrid.SignalName.HolderAltPressed,
			Callable.From((Action<NCardHolder>)(h => ShowCardDetail(h.CardModel!))));
		_grid.InsetForTopBar();

		_ = (long)_peekButton.Connect(NPeekButton.SignalName.Toggled, Callable.From((Action<NPeekButton>)(_ =>
		{
			if (_peekButton.IsPeeking)
			{
				MouseFilter = MouseFilterEnum.Ignore;
			}
			else
			{
				MouseFilter = MouseFilterEnum.Stop;
				ActiveScreenContext.Instance.Update();
			}
		})));
		Callable.From(SetPeekButtonTargets).CallDeferred();
	}

	private void RefreshGrid(IReadOnlyList<CardModel> cardsToShow)
	{
		if (_grid == null) return;
		const int num1 = 1;
		var sortingOrdersList = new List<SortingOrders>(num1);
		CollectionsMarshal.SetCount(sortingOrdersList, num1);
		CollectionsMarshal.AsSpan(sortingOrdersList)[0] = SortingOrders.Ascending;
		_grid.SetCards(cardsToShow, PileType.None, sortingOrdersList);
	}

	public static NGemUpgradeSelectScreen Create(
		IReadOnlyList<CardModel> gems,
		IReadOnlyList<CardModel> gemHolder,
		CardSelectorPrefs prefs)
	{
		var cardSelectScreen = PreloadManager.Cache.GetScene(ScenePath)
			.Instantiate<NGemUpgradeSelectScreen>();
		cardSelectScreen.Name = nameof(NGemUpgradeSelectScreen);
		cardSelectScreen._gems = gems;
		cardSelectScreen._cards = gemHolder;
		cardSelectScreen._prefs = prefs;
		return cardSelectScreen;
	}

	private void RefreshConfirmButtonVisibility()
	{
		if (_confirmButton == null) return;
		if (_selectedCards.Count == 1)
			_confirmButton.Enable();
		else
			_confirmButton.Disable();
	}

	private void OnCardClicked(CardModel card)
	{
		if (_grid == null) return;
		// Toggle selection
		if (_selectedCards.Contains(card))
		{
			_selectedCards.Remove(card);
			_grid.UnhighlightCard(card);
		}
		else
		{
			// Enforce single selection by clearing previous highlights
			foreach (var previouslySelected in _selectedCards) _grid.UnhighlightCard(previouslySelected);
			_selectedCards.Clear();

			_selectedCards.Add(card);
			_grid.HighlightCard(card);
		}

		RefreshConfirmButtonVisibility();
	}

	private void ConfirmSelection(NButton _)
	{
		if (_cards == null) return;
		if (_isSelectingGem)
		{
			_selectedGem = _selectedCards.First();
			_selectedCards.Clear();
			_isSelectingGem = false;
			RefreshGrid(_cards);
			RefreshConfirmButtonVisibility();

			// Todo: localization
			if (_infoLabel == null) return;
			_infoLabel.Text = "Select a Card to socket the Gem.";
		}
		else
		{
			CheckIfSelectionComplete();
		}
	}

	private void CheckIfSelectionComplete()
	{
		if (_selectedCards.Count == 0 || NOverlayStack.Instance == null || _selectedGem == null)
			return;

		// Return both the chosen Gem and the chosen Holder
		var finalSelection = new List<CardModel> { _selectedGem, _selectedCards.First() };

		CompletionSource.SetResult(finalSelection);
		NOverlayStack.Instance.Remove(this);
	}

	private void CloseSelection(NButton _)
	{
		if (NOverlayStack.Instance == null) return;
		CompletionSource.SetResult(Array.Empty<CardModel>());
		NOverlayStack.Instance.Remove(this);
	}

	public override void _ExitTree()
	{
		if (CompletionSource.Task.IsCompleted)
			return;
		CompletionSource.SetCanceled();
	}

	private void SetPeekButtonTargets()
	{
		if (_peekButton == null || _grid == null) return;
		var source = new HashSet<Control> { _grid };
		source.UnionWith(PeekButtonTargets);
		_peekButton.AddTargets(source.ToArray());
	}

	private void ShowCardDetail(CardModel card)
	{
		if (NControllerManager.Instance == null || NGame.Instance == null || _grid == null) return;
		if (NControllerManager.Instance.IsUsingController)
			return;

		// Use correct context list depending on phase
		var listToInspect = _isSelectingGem ? _gems : _cards;
		if (listToInspect == null) return;
		NGame.Instance.GetInspectCardScreen().Open(listToInspect.ToList(), listToInspect.ToList().IndexOf(card),
			_grid.IsShowingUpgrades);
	}
}
