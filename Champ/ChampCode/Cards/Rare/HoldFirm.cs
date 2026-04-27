using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class HoldFirm : ChampCardModel
{
    public HoldFirm() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(10, 3);
        WithPower<CounterPower>(10, 3);
        WithPower<BlurPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<CounterPower>(ctx, this);
        await CommonActions.ApplySelf<BlurPower>(ctx, this);
    }
}