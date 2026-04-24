using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Guardian.Basic;

[Pool(typeof(GuardianCardPool))]
public class CurlUp : GuardianCardModel
{
    public CurlUp() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBrace(10, 2);
        WithTip(DownfallTip.Stasis);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        if (GuardianCmd.CanPutIntoStasis(Owner))
        {
            CardModel? card;
            if (IsUpgraded)
            {
                var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, 1);
                card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, e => e != this, this)).FirstOrDefault();
            }
            else
            {
                card = PileType.Hand.GetPile(Owner).Cards.Where(e => e != this)
                    .TakeRandom(1, CombatState.RunState.Rng.CombatCardSelection).FirstOrDefault();
            }

            if (card == null) return;
            await GuardianCmd.PutIntoStasis(card, ctx, this);
        }

        await GuardianCmd.Brace(ctx, this);
    }
}