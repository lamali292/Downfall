using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Clang : AutomatonCardModel
{
    public Clang() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(14, 4);
    }

    protected override bool IsPlayable => Owner.GetHand().Any(e => e.Type is CardType.Curse or CardType.Status);

    protected override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
        => CommonActions.CardAttack(this, cardPlay).Execute(ctx);
}