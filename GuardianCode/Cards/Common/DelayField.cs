using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class DelayField : GuardianCardModel
{
    public DelayField() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(12, 4);
        this.WithPolish(1);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await GuardianCmd.Polish(ctx, this);
    }
}