using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Whomp : CollectorCardModel
{
    public Whomp() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(12, 3);
        WithVars(new SummonVar(12).WithUpgrade(3));
        WithKeyword(CardKeyword.Exhaust);
        WithTip(CollectorTip.Kindle);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CollectorCmd.SummonTorchhead(ctx, Owner, DynamicVars.Summon.IntValue, this);
    }
}