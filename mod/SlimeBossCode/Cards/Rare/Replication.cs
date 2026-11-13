using Downfall.DownfallCode.Compatibility;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Replication : SlimeBossCardModel
{
    public Replication() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.PlaySelectionPrompt, 1);
        var card = (await CardSelectCmd.FromCombatPile(ctx, PileType.Draw.GetPile(Owner),
            Owner, prefs, c => c.Type == CardType.Power)).FirstOrDefault();
        if (card == null) return;
        // Play a copy: the chosen Power stays in the Draw Pile.
        await CardCmd.AutoPlay(ctx, card.CreateDupeCompat(), null);
    }
}
