using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class ShadowCloak : HermitCardModel
{
    public ShadowCloak() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<ShadowCloakPower>(4, 2);
        WithTip(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<ShadowCloakPower>(ctx, this);
    }
}