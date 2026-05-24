using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Cards.Basic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class TakeCover : HermitCardModel
{
    public TakeCover() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithUpgradingCardTip<DefendHermit>(WithPreviewModifiers);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await DownfallCardCmd.GiveCard<DefendHermit>(Owner, PileType.Hand, upgraded: IsUpgraded,
            action: card => WithPlayModifiers(card, this));
    }

    private static void WithPreviewModifiers(DefendHermit defend, CardModel cardModel)
    {
        WithModifiers(defend,
            cardModel is { IsMutable: true, _owner: not null } ? cardModel.Owner.PlayerCombatState?.Energy ?? 0 : 3);
    }

    private static void WithPlayModifiers(DefendHermit defend, CardModel cardModel)
    {
        WithModifiers(defend, cardModel.EnergyCost.CapturedXValue);
    }

    private static void WithModifiers(DefendHermit defend, int nimble)
    {
        DownfallCardCmd.ForceUpgrade(defend, nimble);
        defend.SetToFreeThisTurn();
    }
}