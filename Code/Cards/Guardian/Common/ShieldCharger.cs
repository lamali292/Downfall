using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.DynamicVars;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
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
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.PutIntoStasis(this, ctx);
    }
    

    public async Task OnTick(PlayerChoiceContext ctx)
    {
        await GuardianCmd.Brace(this);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
    }
}