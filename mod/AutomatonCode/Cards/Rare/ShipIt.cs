using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class ShipIt : AutomatonCardModel
{
    public ShipIt() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithDamage(10, 4);
        WithCards(2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext,
        CardModel card, bool fromHandDraw)
    {
        if (CombatState == null || card != this) return;
        await CommonActions.Draw(this, choiceContext);
    }
}