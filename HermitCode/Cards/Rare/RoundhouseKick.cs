using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class RoundhouseKick : HermitCardModel
{
    public RoundhouseKick() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(13, 5);
        WithKeyword(CardKeyword.Exhaust);
        this.WithPower<TemporaryStrengthDownPower>(10, 2, false);
        this.WithTip<StrengthPower>();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay); 
        await CommonActions.CardAttack(this, play)
            .WithHermitBluntHeavyHitFx()
            .Execute(ctx);
        await CommonActions.Apply<TemporaryStrengthDownPower>(ctx, this, play);
    }
}