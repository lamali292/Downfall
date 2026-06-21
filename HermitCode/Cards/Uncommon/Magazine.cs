using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public class Magazine : HermitCardModel
{
    public Magazine() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<MagazinePower>(6, 3);
        WithTip(CardKeyword.Retain);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<MagazinePower>(ctx, this);
    }
}