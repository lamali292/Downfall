using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Collector.Common;

[Pool(typeof(CollectorCardPool))]
public class Condemn : CollectorCardModel
{
    public Condemn() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<VulnerablePower>(1, 1);
        WithPower<CollectorDoomPower>(5, 1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VulnerablePower>(this, 1);
        await CommonActions.ApplySelf<CollectorDoomPower>(this, 1);
    }
    
}