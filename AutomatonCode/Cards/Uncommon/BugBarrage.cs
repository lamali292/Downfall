using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class BugBarrage : AutomatonCardModel
{
    public BugBarrage() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithTip(typeof(Error));
        WithCards(2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCards<Error>(Owner, PileType.Hand, DynamicVars.Cards.IntValue);
        var statuses = Owner.GetHand(c => c.Type == CardType.Status);
        var count = statuses.Count;
        await CardCmd.DiscardAndDraw(ctx, statuses, count);
        await CommonActions.CardAttack(this, cardPlay, count).Execute(ctx);
    }
}