using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class EyeOfTheStorm : HermitCardModel
{
    public EyeOfTheStorm() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(HermitKeywords.Concentrate);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var gain = Owner.PlayerCombatState!.MaxEnergy - Owner.PlayerCombatState.Energy;
        await PlayerCmd.GainEnergy(gain, Owner);
    }
}