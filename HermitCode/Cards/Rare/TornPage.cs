using BaseLib.Utils;
using Hermit.HermitCode.Cards.Affliction;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hermit.HermitCode.Cards.Rare;

public class TornPage : HermitCardModel
{
    public TornPage() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(9, 3);
        WithTips(_ => HoverTipFactory.FromAffliction<Necromantic>());
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await CommonActions.CardBlock(this, cardPlay);
        var curses = Owner.GetHand().Where(e => e.Type == CardType.Curse);
        foreach (var cardModel in curses)
        {
            await CardCmd.Afflict<Necromantic>(cardModel, 1);
        }
        
    }
}