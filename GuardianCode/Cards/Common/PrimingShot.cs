using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Extensions;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class PrimingShot : GuardianCardModel, IGemSocketCard
{
    public PrimingShot() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        this.WithBrace(4, 2);
    }

    public int GemSlots => 1;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await GuardianCmd.Brace(ctx, this);
    }
}