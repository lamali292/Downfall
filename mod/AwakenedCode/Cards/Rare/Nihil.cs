using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Nihil : AwakenedCardModel
{
    public Nihil() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<ManaburnPower>(13, 4);
        WithKeyword(AwakenedKeyword.Chant);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.Apply<ManaburnPower>(ctx, this, cardPlay);
        await ChantCmd.Chant(cardPlay, async () =>
        {
            if (CombatState == null) return;
            foreach (var combatStateEnemy in CombatState.HittableEnemies)
            {
                var a = combatStateEnemy.GetInstancedPowerAmountSum<ManaburnPower>();
                if (a <= 0) continue;
                await CompatibilityCreatureCmd.Damage(
                    ctx,
                    combatStateEnemy,
                    a,
                    DamageProps.cardHpLoss,
                    this, cardPlay);
            }
        });
    }
}