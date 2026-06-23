using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class FlashPowder : HermitCardModel
{
    public FlashPowder() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithBlock(5);
        WithKeyword(CardKeyword.Exhaust);
        WithPower<StrengthPower>(-1, -1);
        WithVar("StrengthLoss", 1, 1);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);
        await CommonActions.Apply<StrengthPower>(ctx, this, play);
    }
}