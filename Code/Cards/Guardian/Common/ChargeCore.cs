using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Common;

[Pool(typeof(GuardianCardPool))]
public class ChargeCore : GuardianCardModel, ITickCard
{
    public ChargeCore() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(DownfallKeywords.Volatile);
        WithDamage(10, 5);
    }


    public async Task OnTick(PlayerChoiceContext ctx)
    {
        await CardPileCmd.Draw(ctx, 1, Owner);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await GuardianCmd.PutIntoStasis(this, ctx, this);
    }
}