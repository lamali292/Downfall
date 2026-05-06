using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class HighFrequency : GuardianCardModel
{
    public HighFrequency() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, c => c != this, this)).FirstOrDefault();
        if (card == null) return;

        while (true)
        {
            var a = card.CreateClone();
            await CardPileCmd.Add(a, PileType.Play);
            if (!await GuardianCmd.PutIntoStasis(a, ctx, this, true)) break;
        }

        await CardCmd.Exhaust(ctx, card);
    }
}