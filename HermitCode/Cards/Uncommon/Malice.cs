using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Malice : HermitCardModel
{
    public Malice() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(16, 4);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, null, this)).FirstOrDefault();
        if (card != null) await CardCmd.Exhaust(ctx, card);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();
        if (card?.Type == CardType.Curse)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(CombatState!)
                .WithHermitFireHitFx()
                .Execute(ctx);
        else
            await CommonActions.CardAttack(this, play)
                .WithHermitGunHitFx()
                .Execute(ctx);
    }
}