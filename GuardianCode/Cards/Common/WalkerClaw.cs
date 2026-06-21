using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class WalkerClaw : GuardianCardModel, IGemSocketCard
{
    public WalkerClaw() : base(3, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(26, 6);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    public int GemSlots => 2;
}