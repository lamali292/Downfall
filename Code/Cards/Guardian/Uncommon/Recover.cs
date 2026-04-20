using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.DynamicVars;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Recover : GuardianCardModel
{
    public Recover() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(5, 3);
        WithBrace(3, 1);
        WithTip(DownfallTip.Stasis);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await GuardianCmd.Brace(this);
        if (!GuardianCmd.CanPutIntoStasis(Owner)) return;
        var cards = PileType.Discard.GetPile(Owner).Cards;
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, 1);
        var card = (await CardSelectCmd.FromSimpleGrid(ctx, cards, Owner, prefs)).FirstOrDefault();
        if (card == null) return;
        await GuardianCmd.PutIntoStasis(card, ctx, this);
    }
}