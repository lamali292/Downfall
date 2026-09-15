using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class SlagTeam : CollectorCardModel, IAfterCardPyred
{
    public SlagTeam() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithTorchheadDamage(6, 3);
        WithTip<Ember>();
    }
    
    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CollectorCmd.TorchheadAttack(this, cardPlay).ExecuteIfPresent(ctx);
    }

    public async Task AfterCardPyred(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (pyred is not Ember)
        {
            return;
        }
        if (Pile == null || Pile.Type == PileType.Hand)
        {
            return;
        }
        CardPileAddResult cardPileAddResult = await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("TorchheadTargetsAll", ShouldTorcheadTargetAll);
        base.AddExtraArgsToDescription(description);
    }
}