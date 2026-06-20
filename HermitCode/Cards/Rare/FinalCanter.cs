using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class FinalCanter : HermitCardModel
{
    public FinalCanter() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(10, 3);
        WithCalculatedVar("CalculatedHits", 0, CountCursesInHand);
        WithKeyword(CardKeyword.Retain);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    private static decimal CountCursesInHand(CardModel card, Creature? _)
    {
        return card.Owner.GetHand().Count(c => c.Type == CardType.Curse);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        var hitCount = (int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(play.Target);
        if (hitCount <= 0)
            return;
        await CommonActions.CardAttack(this, play, hitCount)
            .WithHermitFireHitFx()
            .Execute(ctx);
    }
}