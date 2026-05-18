using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class DoubleStyle : ChampCardModel
{
    public DoubleStyle() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<DefensiveStylePower>(1, 1, false);
        WithPower<BerserkerStylePower>(1, 1, false);
        WithTip(typeof(VigorPower));
        WithTip(typeof(CounterPower));
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DefensiveStylePower>(ctx, this);
        await CommonActions.ApplySelf<BerserkerStylePower>(ctx, this);
    }
}
