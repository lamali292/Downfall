using BaseLib.Patches.Features;
using BaseLib.Utils;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Curse;

[Pool(typeof(CurseCardPool))]
public sealed class ImpendingDoom : HermitCardModel, IHasDeadOnEffect
{
    public ImpendingDoom() : base(-2, CardType.Curse, CardRarity.Curse, CustomTargetType.Everyone)
    {
        WithVar(new DamageVar(13, ValueProp.Move | ValueProp.Unpowered));
        WithKeyword(CardKeyword.Unplayable);
    }

    public override int MaxUpgradeLevel => 0;


    protected override bool ShouldGlowGoldInternal => false;
    protected override bool ShouldGlowRedInternal => IsDeadOn;
    public override bool HasTurnEndInHandEffect => IsDeadOn;

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
        var cardPlay = new CardPlay
        {
            Card = this,
            Target = null,
            ResultPile = PileType.Discard,
            Resources = default,
            IsAutoPlay = true,
            PlayIndex = 0,
            PlayCount = 1
        };
        await HermitCmd.TriggerDeadOnEffect(ctx, this, cardPlay);
    }
}