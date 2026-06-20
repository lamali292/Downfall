using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Commit : AutomatonCardModel
{
    public Commit() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithBlock(6, 2);
        WithDamage(6, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedFunction;

    private bool WasLastCardPlayedFunction => CombatManager.Instance.History.CardPlaysFinished
        .LastOrDefault(e =>
            e.CardPlay.Card.Owner == Owner &&
            e.CardPlay.Card != this)?.CardPlay.Card is FunctionCard;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card == this && WasLastCardPlayedFunction)
        {
            modifiedCost = 0;
            return true;
        }

        modifiedCost = originalCost;
        return false;
    }
}