using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gremlins.GremlinsCode.Cards.Common;

[Pool(typeof(GremlinsCardPool))]
public class ForTheLeader : GremlinsCardModel
{
    public ForTheLeader() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        WithPower<ForTheLeaderPower>(1, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.ApplySelf<ForTheLeaderPower>(ctx, this);
    }
}

public class ForTheLeaderPower : CustomTemporaryPowerModelWrapper<ForTheLeader, StrengthPower>;