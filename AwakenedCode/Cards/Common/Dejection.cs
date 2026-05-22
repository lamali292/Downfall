using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Dejection : AwakenedCardModel
{
    public Dejection() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
        WithTip(CardKeyword.Exhaust);
        WithTip(typeof(Ceremony));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var selected = (await DownfallCardCmd.SelectFromHand(ctx, CardSelectorPrefs.ExhaustSelectionPrompt, this))
            .FirstOrDefault();
        if (selected == null) return;
        await CardCmd.Exhaust(ctx, selected);
        if (selected is ISpell) await DownfallCardCmd.GiveCard<Ceremony>(Owner, PileType.Hand);
    }
}