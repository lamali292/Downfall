using BaseLib.Utils;
using Gremlins.GremlinsCode.Core;
using Gremlins.GremlinsCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gremlins.GremlinsCode.Cards.Rare;

[Pool(typeof(GremlinsCardPool))]
public class Nob : GremlinsCardModel
{
    public Nob() : base(4, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithTempHp(20, 10);
        WithPower<NobPower>(1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GremlinsCmd.GainTempHp(ctx, this);
        await CommonActions.ApplySelf<NobPower>(ctx, this);
    }
}