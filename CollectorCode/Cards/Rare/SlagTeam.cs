using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class SlagTeam : CollectorCardModel, IAfterCardPyred
{
    public SlagTeam() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithTorchheadDamage(7, 3);
        WithTip(CollectorTip.Pyred);
        WithTip(CollectorKeyword.Pyre);
        WithTip(CardKeyword.Exhaust);
        WithCardTip<Ember>();
        WithTip(CollectorKeyword.Torchhead);
    }
    
    
    /// <summary>Torchhead hits what you pick; while it targets ALL enemies (Equip Axe) there is nothing to pick.</summary>
    public override TargetType TargetType => _owner != null && IsMutable && ShouldTorcheadTargetAll
        ? TargetType.AllEnemies
        : TargetType.AnyEnemy;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await TorchheadCmd.Attack(ctx, this, cardPlay);
    }

    public async Task AfterCardPyred(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (pyred is not Ember || pyred.Owner != Owner || card.Owner != Owner) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }
}