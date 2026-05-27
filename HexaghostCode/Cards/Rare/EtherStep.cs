using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class EtherStep : HexaghostCardModel, IHasAfterlifeEffect
{
    public EtherStep() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        this.WithAfterlife();
        WithDamage(10, 4);
        WithCards(1, 1);
        WithTip(CardKeyword.Exhaust);
    }

    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AfterlifeEffect(ctx, cardPlay);

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1, 1);
        var exhausted = (await CardSelectCmd.FromHand(ctx, Owner, prefs, e => e != this, this)).FirstOrDefault();
        if (exhausted != null) await CardCmd.Exhaust(ctx, exhausted);
        await CommonActions.Draw(this, ctx);
    }
}