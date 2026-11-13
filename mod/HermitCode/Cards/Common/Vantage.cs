using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class Vantage : HermitCardModel
{
    public Vantage() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 2);
        WithCards(1, 1);
        WithDeadOn();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);
        await HermitCmd.DeadOn(ctx, this, play, async () =>
        {
            (await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner))
                .Where(e => e.IsUpgradable)
                .ToList()
                .ForEach(card => CardCmd.Upgrade(card));
        });
    }
}