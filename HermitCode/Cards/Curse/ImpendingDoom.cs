using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.CustomEnums;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Curse;

[Pool(typeof(CurseCardPool))]
public sealed class ImpendingDoom : HermitCardModel
{
    public ImpendingDoom() : base(-2, CardType.Curse, CardRarity.Curse, DownfallTargetType.MeAndEnemies)
    {
        WithVar(new DamageVar(13, DamageProps.cardUnpowered));
        WithKeyword(CardKeyword.Unplayable);
        WithDeadOn();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public override int MaxUpgradeLevel => 0;


    protected override bool ShouldGlowGoldInternal => false;
    protected override bool ShouldGlowRedInternal => HermitCmd.HasActiveDeadOnEffect(this);

    // The game moves this card into the Play pile before calling OnTurnEndInHand, so this getter
    // (read while the card is still in Hand) is the last chance to snapshot Dead On status for
    // HermitCmd.IsDeadOn to find afterward - see DeadOnPatch.CaptureNow.
    public override bool HasTurnEndInHandEffect
    {
        get
        {
            DeadOnPatch.CaptureNow(this);
            return HermitCmd.HasActiveDeadOnEffect(this);
        }
    }

    public override bool CanBeGeneratedByModifiers => false;

    private static bool IsMultiplayer => (RunManager.Instance.DebugOnlyGetState()?.Players.Count ?? 1) > 1;

    protected override async Task OnTurnEndInHand(PlayerChoiceContext ctx)
    {
        await HermitCmd.DeadOn(ctx, this, null, async () =>
        {
            var targets = CombatState!.Creatures.Where(e => e is { IsHittable: true, IsPet: false }).ToList();
            foreach (var child in targets.Select(target => NFireBurstVfx.Create(target, 0.75f)!))
            {
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child);
            }

            await CreatureCmd.Damage(ctx, targets, DynamicVars.Damage, Owner.Creature, this, null);
        });
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("Multiplayer", IsMultiplayer);
    }
}