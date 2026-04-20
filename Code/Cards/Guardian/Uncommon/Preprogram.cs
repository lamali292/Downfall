using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Preprogram : GuardianCardModel
{
    protected override int GemSlots => 1;
    public Preprogram() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(5, 3);
        WithTip(DownfallTip.Stasis);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (!GuardianCmd.CanPutIntoStasis(Owner)) return;
        var cards = PileType.Draw.GetPile(Owner).Cards.Take(DynamicVars.Cards.IntValue).ToList();
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, 1);
        var card = (await CardSelectCmd.FromSimpleGrid(ctx, cards, Owner, prefs)).FirstOrDefault();
        if (card == null) return;
        await GuardianCmd.PutIntoStasis(card, ctx, this);
    }
}