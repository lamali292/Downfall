using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class ChargeUp : GuardianCardModel, IGemSocketCard
{
    public ChargeUp() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 2);
        this.WithPower<NextTurnTemporaryStrengthUpPower>(2, 1, false);
        WithTip(typeof(StrengthPower));
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public int GemSlots => 1;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<NextTurnTemporaryStrengthUpPower>(ctx, this);
    }
}