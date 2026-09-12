using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Collector.CollectorCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Blightning : CollectorCardModel
{
    public Blightning() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithKindle(1, 2);
        WithTorchheadDamage(9, 2);
        WithPower<MiasmaPower>(3, 2);
        WithCards(2);
        WithKeyword(CardKeyword.Exhaust);
        WithTags(CardTag.Strike);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await CollectorCmd.Kindle(ctx, this);
        await CollectorCmd.TorchheadAttack(this).ExecuteIfPresent(ctx);
        //await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<MiasmaPower>(ctx, this, cardPlay);
        await CommonActions.Draw(this, ctx);

    }
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        var shouldTargetAll = _owner != null && CollectorHook.ShouldTorchheadTargetAll(_owner, out _);
        description.Add("TorchheadTargetsAll", shouldTargetAll);
        base.AddExtraArgsToDescription(description);
    }
}