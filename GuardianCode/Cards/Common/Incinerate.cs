using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Extensions;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class Incinerate : GuardianCardModel, IGemSocketCard
{
    public Incinerate() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8);
        this.WithAccelerate(1, 1);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public int GemSlots => 1;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await GuardianCmd.Accelerate(ctx, this);
    }
}