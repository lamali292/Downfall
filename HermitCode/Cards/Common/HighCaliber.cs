using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Cards.Basic;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Common;

public sealed class HighCaliber : HermitCardModel
{
    public HighCaliber() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithKeyword(CardKeyword.Exhaust);
        WithUpgradingCardTip<StrikeHermit>(WithModifiers);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play).WithHermitGunHitFx().BeforeDamage(() =>
            {
                HermitSfx.PlayGun1();
                return Task.CompletedTask;
            })
            .Execute(ctx);
        await DownfallCardCmd.GiveCard<StrikeHermit>(Owner, PileType.Hand, upgraded: IsUpgraded,
            action: card => WithModifiers(card, this));
    }

    private static void WithModifiers(StrikeHermit strike, CardModel card)
    {
        DownfallCardCmd.ForceUpgrade(strike, 2);
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        var level = CurrentUpgradeLevel + 2;
        switch (level)
        {
            case 0:
                description.Add("UpgradeAmount", "");
                break;
            case 1:
                description.Add("UpgradeAmount", "+");
                break;
            case > 1:
                description.Add("UpgradeAmount", $"+{level}");
                break;
        }
    }
}