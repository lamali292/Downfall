using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class SludgeBomb : AwakenedCardModel
{
    public SludgeBomb() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(18, 4);
    }

    protected override bool IsPlayable => PileType.Exhaust.GetPile(Owner).Cards.Any(c => c is Void);


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = PileType.Exhaust.GetPile(Owner).Cards.FirstOrDefault(c => c is Void);
        if (card == null) return;
        await CardPileCmd.RemoveFromCombat(card);
    }
}