using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class TimeBomb : GuardianCardModel
{
    public TimeBomb() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(14, 4);
        WithKeyword(CardKeyword.Exhaust);
        WithAccelerate(1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await GuardianCmd.Accelerate(this, AccelerateType.All);
    }
}