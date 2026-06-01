using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Extensions;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class ShieldCharger : GuardianCardModel, ITickCard
{
    public ShieldCharger() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(10, 2);
        WithKeyword(GuardianKeyword.Volatile);
        WithTip(GuardianTip.Stasis);
        this.WithBrace(4, 2);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();


    public async Task OnTick(PlayerChoiceContext ctx)
    {
        await GuardianCmd.Brace(ctx, this);
        await CommonActions.CardBlock(this, null);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.PutIntoStasis(this, ctx);
    }
}