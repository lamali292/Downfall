using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Multiplayer;

public class RubberBullet : HermitCardModel
{
    public RubberBullet() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(7, 2);
        WithVar("Increase", 7, 2);
        WithDeadOn();
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override Artist Artist => Artist.Get<Fulgur>();
    protected override bool IsBetaArt => true;

    
    // Runs before this card's own effect (see CardModel.OnPlayWrapper), same as every other
    // ModifyCardPlayResultLocation redirect (Combo, Feral, Reroute, ...) - so this composes with
    // them generically instead of RubberBullet needing to know about any of them by name: if
    // something else already committed this exact card to the owner's own hand (e.g. Combo), that
    // wins and we back off, whichever order the hooks happen to run in.
    public override CardLocation ModifyCardPlayResultLocation(CardModel card, bool isAutoPlay,
        ResourceInfo resources, CardLocation cardLocation)
    {
        if (card != this || !HermitCmd.HasActiveDeadOnEffect(card) || cardLocation is { pileType: PileType.Hand, player: var p } && p == card.Owner) return cardLocation;
        var teammate = Owner.RandomOtherTeammate;
        return teammate == null ? cardLocation : new CardLocation(teammate, PileType.Hand, CardPilePosition.Bottom);
    }

    public override Task AfterModifyingCardPlayResultLocation(CardModel card, CardLocation cardLocation)
    {
        if (card == this && cardLocation.player != card.Owner)
            HermitSfx.PlayReload();
        return Task.CompletedTask;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, cardPlay).WithHermitGunHitFx().BeforeDamage(() =>
            {
                HermitSfx.PlayGun3();
                return Task.CompletedTask;
            })
            .Execute(ctx);
        // Upgrades this card instance's damage for its *next* play, not this one - matches the
        // redirect-to-teammate-hand effect above, which the game already committed before this runs.
        await HermitCmd.DeadOn(ctx, this, cardPlay,
            () =>
            {
                DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].IntValue);
                return Task.CompletedTask;
            });
    }
}
