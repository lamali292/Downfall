using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Goodbye : CollectorCardModel
{
    public Goodbye() : base(5, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithCostUpgradeBy(-1);
        WithKeyword(CardKeyword.Retain);
        WithKeyword(CollectorKeyword.Megapyre);
        WithTip(CollectorTip.Pyred);
        WithKeyword(CardKeyword.Exhaust);
        WithTip<MiasmaPower>();
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var pyred = await PyreCmd.MegaPyre(ctx, this);
        var enemies = CombatState.HittableEnemies;
        foreach (var e in enemies)
        {
            var powerAmount = e.GetPowerAmount<MiasmaPower>();
            if (powerAmount <= 0)
                continue;
            await PowerCmd.Apply<MiasmaPower>(ctx, e, pyred.Count, Owner.Creature, this);
        }
    }
}