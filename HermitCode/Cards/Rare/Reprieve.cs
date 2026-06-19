using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class Reprieve : HermitCardModel
{
    public override bool CanBeGeneratedInCombat => false;
    public Reprieve() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithHeal(10, 3);
        WithKeyword(CardKeyword.Ethereal);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
}