using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class OrbSupport : GuardianCardModel
{
    public OrbSupport() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 3);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(DownfallTip.Brace);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var attack = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var unblocked = attack.Results.Sum(e => e.UnblockedDamage);
        await GuardianCmd.Brace(Owner, unblocked);
    }
}