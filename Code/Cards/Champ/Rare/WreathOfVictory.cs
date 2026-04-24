using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class WreathOfVictory : ChampCardModel
{
    public WreathOfVictory() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<VigorPower>(6, 2);
        WithPower<CounterPower>(6, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VigorPower>(ctx, this);
        await CommonActions.ApplySelf<CounterPower>(ctx, this);
    }
}