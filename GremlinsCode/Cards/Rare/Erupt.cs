using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gremlins.GremlinsCode.Cards.Rare;

[Pool(typeof(GremlinsCardPool))]
public class Erupt : GremlinsCardModel
{
    public Erupt() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<EruptPower>(5, 2);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<EruptPower>(ctx, this);
    }
}

public class EruptPower : CustomTemporaryPowerModelWrapper<Erupt, StrengthPower>;