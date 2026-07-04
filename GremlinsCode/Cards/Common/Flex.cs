using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gremlins.GremlinsCode.Cards.Common;

[Pool(typeof(GremlinsCardPool))]
public class Flex : GremlinsCardModel
{
    public Flex() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<FlexPower>(2, 2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<FlexPower>(ctx, this);
    }
}

public class FlexPower : CustomTemporaryPowerModelWrapper<Flex, StrengthPower>;