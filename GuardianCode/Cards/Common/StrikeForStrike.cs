using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class StrikeForStrike : GuardianCardModel, IGemSocketCard
{
    public StrikeForStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithCalculatedDamage(13, Calc, DamageProps.card, 4);
        WithVars(new DamageVar("SelfDamage", 3, DamageProps.cardUnpowered));
        WithTags(CardTag.Strike);
    }

    public int GemSlots => 1;

    private static decimal Calc(CardModel card, Creature? _)
    {
        return card.Owner.Creature.GetPowerAmount<ThornsPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var damage = (DamageVar)DynamicVars["SelfDamage"];
        await CompatibilityCreatureCmd.Damage(ctx, Owner.Creature, damage.BaseValue, damage.Props, this, cardPlay);
    }
}