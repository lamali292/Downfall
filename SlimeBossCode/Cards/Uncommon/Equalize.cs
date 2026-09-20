using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Interfaces;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Equalize : SlimeBossCardModel, IHasConsumeEffect
{
    private bool _consumedThisPlay;

    public Equalize() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 4);
        WithHeal(4, 2);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(SlimeBossTip.Consume);
    }

    public override bool CanBeGeneratedInCombat => false;

    protected override Artist Artist => Artist.Get<Opal>();


    public Task ConsumeEffect(PlayerChoiceContext ctx, Creature creature, AttackCommand command, int amount)
    {
        _consumedThisPlay = true;
        return Task.CompletedTask;
    }

    // "Consume: Play this twice" is implemented by repeating the attack+heal in place rather than
    // recursively calling CardCmd.AutoPlay(this): AutoPlay re-enters this same CardModel's still-running
    // OnPlayWrapper (and, outside of tests, the still-in-flight NCard/pile visuals for the original manual
    // play), which corrupted the card's play/pile state and crashed on a later play.
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        _consumedThisPlay = false;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);

        if (_consumedThisPlay)
        {
            _consumedThisPlay = false;

            // If the first hit already killed the last enemy, combat starts ending right there, and
            // the attack would have no valid target (vanilla's own multi-play loop in
            // CardModel.OnPlayWrapper checks this before every extra iteration and stops for the same
            // reason). The heal still applies regardless: it targets the player, and CreatureCmd.Heal
            // explicitly still heals players even once combat IsEnding, so that part of "play this
            // twice" remains worth it.
            if (!CombatManager.Instance.IsOverOrEnding)
                await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        }
    }
}