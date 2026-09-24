using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class SlagTeam : CollectorCardModel, IAfterCardPyred
{
    public SlagTeam() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithTorchheadDamage(7, 4);
        WithTip(CollectorTip.Pyred);
        WithTip(CollectorKeyword.Pyre);
        WithTip(CardKeyword.Exhaust);
        WithCardTip<Ember>();
        //WithCardTip<Soot>();
    }
    
    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CollectorCmd.TorchheadAttack(this, cardPlay).ExecuteIfPresent(ctx);
        //await DownfallCardCmd.GiveCard<Soot>(Owner, PileType.Hand);
    }

    public async Task AfterCardPyred(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (pyred is not Ember || pyred.Owner != Owner || card.Owner != Owner) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("TorchheadTargetsAll", ShouldTorcheadTargetAll);
        base.AddExtraArgsToDescription(description);
    }
}