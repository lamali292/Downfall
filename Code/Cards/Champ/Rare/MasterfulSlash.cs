using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class MasterfulSlash : ChampCardModel
{
    public MasterfulSlash() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(9, 3);
        WithTip(typeof(VigorNextTurnPower));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var attack = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var unblocked = attack.Results.Sum(r => r.UnblockedDamage);
        await CommonActions.ApplySelf<VigorNextTurnPower>(this, unblocked);
    }
}