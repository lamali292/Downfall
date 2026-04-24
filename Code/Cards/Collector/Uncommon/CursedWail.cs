using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Downfall;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class CursedWail : CollectorCardModel
{
    public CursedWail() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithPower<TemporaryStrengthDownPower>(9, 2);
        WithPower<StrengthPower>(1, 1);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CommonActions.Apply<TemporaryStrengthDownPower>(ctx,CombatState.Enemies, this);
        ;
        var amount = -DynamicVars.Power<StrengthPower>().IntValue;
        await PowerCmd.Apply<StrengthPower>(ctx, CombatState.Enemies.Where(e => e.IsAfflicted()), amount, Owner.Creature,
            this);
    }
}