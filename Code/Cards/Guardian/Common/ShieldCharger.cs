using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Common;

[Pool(typeof(GuardianCardPool))]
public class ShieldCharger : GuardianCardModel, ITickCard
{
    public ShieldCharger() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(10, 2);
        WithKeyword(DownfallKeywords.Volatile);
        WithBrace(4, 2);
    }


    public async Task OnTick(PlayerChoiceContext ctx)
    {
        await GuardianCmd.Brace(ctx, this);
        await CommonActions.CardBlock(this, null);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.PutIntoStasis(this, ctx);
    }
}