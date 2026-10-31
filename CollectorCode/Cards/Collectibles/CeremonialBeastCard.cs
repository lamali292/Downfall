using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Collectibles;

public class CeremonialBeastCard : Collectible<CeremonialBeastBoss>
{
    public CeremonialBeastCard() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self, 0.72f)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCalculatedVar("StrengthCalc", 0, 1, StrengthCalc);
        WithPower<CeremonialBeastCardPower>(1, false);
        WithTip<StrengthPower>();
    }

    /// <summary>Strength equals your Energy, and once upgraded also your Reserve.</summary>
    private static decimal StrengthCalc(CardModel card, Creature? creature)
    {
        var combatState = card.Owner.PlayerCombatState;
        var reserve = card.IsUpgraded ? combatState?.Reserve ?? 0 : 0;
        return (combatState?.Energy ?? 0) + reserve;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var strength = (int)((CalculatedVar)DynamicVars["StrengthCalc"]).Calculate(null);
        if (strength > 0)
            await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, strength, Owner.Creature, this);
        await CommonActions.ApplySelf<CeremonialBeastCardPower>(ctx, this);
    }
}