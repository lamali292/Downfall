using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Spite : HermitCardModel
{
    public Spite() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(8, 2);
        WithCards(3, 1);
        WithTip(CardKeyword.Unplayable);
        WithTip(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await Owner.GetHand()
            .Where(c => c.Keywords.Contains(CardKeyword.Unplayable))
            .ForEachAsync(card => CardCmd.Exhaust(ctx, card));
        await CommonActions.CardBlock(this, play);
        await CommonActions.Draw(this, ctx);
    }
}