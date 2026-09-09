using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class MiniCurse : CollectorCardModel
{
    public MiniCurse() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithPower<WeakPower>(1);
        WithPower<VulnerablePower>(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public override TargetType TargetType => IsUpgraded ? TargetType.AllEnemies : TargetType.AnyEnemy;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
    }
}