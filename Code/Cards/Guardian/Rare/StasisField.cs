using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class StasisField : GuardianCardModel
{
    public StasisField() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(6, 2);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await GuardianCmd.PutIntoStasis(this, ctx);
    }
}