using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class ArenaPreparation : ChampCardModel
{
    public ArenaPreparation() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithKeywords(CardKeyword.Exhaust);
        WithTip(CardKeyword.Retain);
        WithCards(2);
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.RetainSelectionPrompt, DynamicVars.Cards.IntValue);
        var cards = await CardSelectCmd.FromHand(ctx, Owner, prefs, c => !c.Keywords.Contains(CardKeyword.Retain),
            this);
        foreach (var cardModel in cards) CardCmd.ApplyKeyword(cardModel, CardKeyword.Retain);
    }
}