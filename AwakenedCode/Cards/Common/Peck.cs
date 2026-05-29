using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Peck : AwakenedCardModel
{
    public Peck() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithBlock(5, 1);
        WithDamage(1);
        WithCards(1, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
    }
}