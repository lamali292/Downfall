using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class SludgeBomb : AwakenedCardModel
{
    public SludgeBomb() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithTip(CardKeyword.Exhaust);
        this.WithTip<Void>();
        WithDamage(18, 4);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override bool IsPlayable => Owner.GetExhaust().Any(c => c is Void);


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        //var card = Owner.GetExhaust().FirstOrDefault(c => c is Void);
        //if (card == null) return;
        //await CardPileCmd.RemoveFromCombat(card);
    }
}