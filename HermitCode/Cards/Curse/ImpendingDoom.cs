using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.CustomEnums;
using Hermit.HermitCode.Core;
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
public sealed class ImpendingDoom : HermitCardModel, IHasDeadOnEffect
{
    public ImpendingDoom() : base(-2, CardType.Curse, CardRarity.Curse, DownfallTargetType.MeAndEnemies)
    {
        WithVar(new DamageVar(13, DamageProps.cardUnpowered));
        WithKeyword(CardKeyword.Unplayable);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public override int MaxUpgradeLevel => 0;


    protected override bool ShouldGlowGoldInternal => false;
    protected override bool ShouldGlowRedInternal => HermitCmd.HasActiveDeadOnEffect(this);
    public override bool HasTurnEndInHandEffect => HermitCmd.HasActiveDeadOnEffect(this);
    public override bool CanBeGeneratedByModifiers => false;

    private static bool IsMultiplayer => (RunManager.Instance.State?.Players.Count ?? 1) > 1;

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var targets = CombatState!.Creatures.Where(e => e is { IsAlive: true, IsPet: false });
        foreach (var target in targets)
        {
            var child = NFireBurstVfx.Create(target, 0.75f)!;
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child);
        }

        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }


    protected override async Task OnTurnEndInHand(PlayerChoiceContext ctx)
    {
        var cardPlay = CardPlayCompat.Create(this, null, PileType.Discard, new ResourceInfo
        {
            EnergySpent = 0,
            EnergyValue = 0,
            StarsSpent = 0,
            StarValue = 0
        }, playCount: 1);
        await HermitCmd.TriggerDeadOnEffect(ctx, this, cardPlay);
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("Multiplayer", IsMultiplayer);
    }
}