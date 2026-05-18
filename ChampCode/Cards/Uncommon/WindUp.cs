using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class WindUp : ChampCardModel
{
    public WindUp() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithTip(ChampTip.Stance);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.SelectStanceToEnter(ctx, Owner);
        var cards = PileType.Draw.GetPile(Owner).Cards.Where(c => c.Tags.Contains(ChampTag.Finisher)).ToList();
        if (cards.Count == 0) return;

        CardModel? card;
        if (cards.Count == 1)
        {
            card = cards.First();
        }
        else
        {
            var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, 1);
            card = (await CardSelectCmd.FromSimpleGrid(ctx, cards, Owner, prefs)).FirstOrDefault();
        }

        if (card == null) return;
        await CardPileCmd.Add(card, PileType.Hand);
    }
}