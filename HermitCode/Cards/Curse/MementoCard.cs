using BaseLib.Patches.Features;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Curse;

[Pool(typeof(CurseCardPool))]
public sealed class MementoCard : HermitCardModel
{
    public MementoCard() : base(0, CardType.Curse, CardRarity.Curse, CustomTargetType.Everyone)
    {
        WithPower<VulnerablePower>(1);
        WithKeyword(CardKeyword.Retain);
    }

    public override int MaxUpgradeLevel => 0;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.Apply<VulnerablePower>(ctx, this, play);
    }
}