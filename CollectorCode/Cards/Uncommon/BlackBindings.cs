using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class BlackBindings : CollectorCardModel
{
    public BlackBindings() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(2);
        WithCalculatedVar("Miasma", 0, 3, Calc, 0, 1);
        WithTip<MiasmaPower>();
    }

    private static decimal Calc(CardModel card, Creature? creature)
    {
        if (creature == null) return 0;
        var amount = creature.Powers.Count(ShouldCountPower);
        if (!creature.Powers.Any(e => e is WeakPower)) amount++;
        return amount;
    }
    
    
    private static bool ShouldCountPower(PowerModel power)
    {
        return power.TypeForCurrentAmount == PowerType.Debuff && power is not ITemporaryPower;
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await CommonActions.Apply<WeakPower>(ctx, cardPlay.Target, this);
        var value = ((CalculatedVar)DynamicVars["Miasma"]).Calculate(cardPlay.Target);
        await CommonActions.Apply<MiasmaPower>(ctx, cardPlay.Target, this, value);
    }
}