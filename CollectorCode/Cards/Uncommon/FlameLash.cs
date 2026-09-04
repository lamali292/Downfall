using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class FlameLash : CollectorCardModel, IHasPyre
{
    public FlameLash() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithDamage(8, 4);
    }

    private bool isAoE = false;
    protected override Artist Artist => Artist.Get<Opal>();

    public CardModel? PyredCard { get; set; }

    public override TargetType TargetType => (_owner == null || !IsMutable ? TargetType.AnyEnemy :
        isAoE ? TargetType.AllEnemies : TargetType.AnyEnemy);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        isAoE = (PyredCard!.EnergyCost.GetResolved() > 2);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}