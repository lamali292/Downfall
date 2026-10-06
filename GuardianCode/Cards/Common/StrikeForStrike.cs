using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
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
        WithCalculatedDamage(13,  Calc, DamageProps.card, 4);
        WithVars(new DamageVar("SelfDamage", 3, DamageProps.cardUnpowered));
        WithTags(CardTag.Strike);
    }

    private static decimal Calc(CardModel card, Creature? _)
    {
        return card.Owner.Creature.GetPowerAmount<ThornsPower>();
    }

    public int GemSlots => 1;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var damage = (DamageVar)DynamicVars["SelfDamage"];
        await CreatureCmd.Damage(ctx, Owner.Creature, damage, this, cardPlay);
    }
}