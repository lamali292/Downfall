using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Clone : GuardianCardModel
{
    public Clone() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithAccelerate(1);
        WithTip(DownfallTip.Stasis);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, e => e != this, this)).FirstOrDefault();
        if (card == null) return;
        var clone = card.CreateClone();
        await GuardianCmd.PutIntoStasis(clone, ctx, this);
        if (IsUpgraded)
            await GuardianCmd.Accelerate(this);
    }
}