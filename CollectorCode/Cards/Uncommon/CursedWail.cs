using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class CursedWail : CollectorCardModel
{
    public CursedWail() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithPower<CursedWailPower>(8, 5, false);
        WithPower<StrengthPower>(1, 1);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        
       
        var amount = -DynamicVars.Power<StrengthPower>().IntValue;
        var enemies = CombatState.HittableEnemies.Where(e => e.Powers.Count(ShouldCountPower) >= 3);
        await PowerCmd.Apply<StrengthPower>(ctx, enemies, amount, Owner.Creature, this);
        
        await CommonActions.Apply<CursedWailPower>(ctx, this, cardPlay);
    }
    
    private static bool ShouldCountPower(PowerModel power)
    {
        return power.TypeForCurrentAmount == PowerType.Debuff && power is not ITemporaryPower;
    }
}

public class CursedWailPower : TemporaryDebuffPowerWrapper<CursedWail, StrengthPower>;