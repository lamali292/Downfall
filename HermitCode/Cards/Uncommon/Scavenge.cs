using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Scavenge : HermitCardModel
{
    public Scavenge() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<PlatedArmorPower>(3, 1);
        WithKeyword(CardKeyword.Exhaust);
        WithVar("DeadOn", 2);
        WithDeadOn();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<PlatedArmorPower>(ctx, this);
        await HermitCmd.DeadOn(ctx, this, play,
            () => CommonActions.ApplySelf<PlatedArmorPower>(ctx, this, DynamicVars["DeadOn"].BaseValue));
    }
}