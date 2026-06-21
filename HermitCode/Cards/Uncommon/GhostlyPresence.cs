using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class GhostlyPresence : HermitCardModel, IHasDeadOnEffect
{
    public GhostlyPresence() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithBlock(8, 2);
        WithPower<WeakPower>(1, 1);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, play);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);
    }
}