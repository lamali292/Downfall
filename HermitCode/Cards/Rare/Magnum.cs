using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class Magnum : HermitCardModel
{
    public Magnum() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithCards(6);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        var handCount = Owner.GetHand().Count;
        var maxDiscard = Math.Min(DynamicVars.Cards.IntValue, handCount);
        if (maxDiscard == 0) return;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, maxDiscard, maxDiscard);
        var selected = (await CardSelectCmd.FromHandForDiscard(
            ctx, Owner, prefs, null, this)).ToList();
        if (selected.Count == 0) return;
        await CardCmd.Discard(ctx, selected);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play, selected.Count).WithHermitGunHitFx().BeforeDamage(() =>
            {
                HermitSfx.PlayGun1();
                return Task.CompletedTask;
            })
            .Execute(ctx);
    }
}