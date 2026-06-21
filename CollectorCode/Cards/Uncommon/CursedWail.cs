using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class CursedWail : CollectorCardModel
{
    public CursedWail() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithPower<TemporaryStrengthDownPower>(9, 2);
        WithPower<StrengthPower>(1, 1);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CommonActions.Apply<TemporaryStrengthDownPower>(ctx, CombatState.Enemies, this);
        ;
        var amount = -DynamicVars.Power<StrengthPower>().IntValue;
        await PowerCmd.Apply<StrengthPower>(ctx, CombatState.Enemies.Where(e => e.IsAfflicted()), amount,
            Owner.Creature,
            this);
    }
}