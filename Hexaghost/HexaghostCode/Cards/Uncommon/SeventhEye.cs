using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class SeventhEye : HexaghostCardModel
{
    public SeventhEye() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner.PlayerCombatState == null) return;
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var card = (await CardSelectCmd.FromSimpleGrid(ctx, Owner.PlayerCombatState.DrawPile.Cards, Owner, prefs))
            .FirstOrDefault();
        if (card != null) await CardPileCmd.Add(card, PileType.Hand);
        await HexaghostCmd.MoveToRandom(ctx, Owner, true);
        await HexaghostCmd.ReplaceCurrentWithRandom(Owner);
    }
}