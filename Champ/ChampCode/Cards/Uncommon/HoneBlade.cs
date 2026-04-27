using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class HoneBlade : ChampCardModel
{
    public HoneBlade() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<HoneBladePower>(3, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<HoneBladePower>(ctx, this);
    }
}