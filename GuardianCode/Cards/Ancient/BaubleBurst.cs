using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Ancient;

[Pool(typeof(GuardianCardPool))]
public class BaubleBurst : GuardianCardModel, IGemSocketCard
{
    public BaubleBurst() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(7);
        this.WithRepeat(3);
    }

    public int GemSlots => IsUpgraded ? 2 : 1;
    public int GemReplayCount => 3;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue).Execute(ctx);
    }
}