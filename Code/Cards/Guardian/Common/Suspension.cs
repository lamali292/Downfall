using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Common;

[Pool(typeof(GuardianCardPool))]
public class Suspension : GuardianCardModel
{
    public Suspension() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6, 3);
    }

    public override int GemSlots => 1;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, e => e != this, this)).FirstOrDefault();
        if (card == null) return;
        await GuardianCmd.PutIntoStasis(card, ctx, this);
    }
}