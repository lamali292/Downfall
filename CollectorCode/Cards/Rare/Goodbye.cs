using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Goodbye : CollectorCardModel
{
    public Goodbye() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithTip<MiasmaPower>();
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var enemies = CombatState.HittableEnemies;
        foreach (var e in enemies)
        {
            var powerAmount = e.GetPowerAmount<MiasmaPower>();
            if (powerAmount <= 0)
                continue;
            await PowerCmd.Apply<MiasmaPower>(ctx, e, powerAmount, Owner.Creature, this);
        }
    }
}