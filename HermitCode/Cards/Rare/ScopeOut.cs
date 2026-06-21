using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Rare;

public class ScopeOut : HermitCardModel
{
    public ScopeOut() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<StrengthPower>(2, 1);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        // todo rest
    }
}