using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Powers.Collector;
using Downfall.Code.Powers.Downfall;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Karma : CollectorCardModel
{
    public Karma() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<KarmaPower>(2, 1);
        WithPower<MetallicizePower>(2, 1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MetallicizePower>(this);
        await CommonActions.ApplySelf<KarmaPower>(this);
    }
}

