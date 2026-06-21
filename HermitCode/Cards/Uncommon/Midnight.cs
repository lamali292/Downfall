using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Cards.Curse;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Midnight : HermitCardModel
{
    public Midnight() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(12, 3);
        this.WithTip<ImpendingDoom>();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);
        await DownfallCardCmd.GiveCard<ImpendingDoom>(Owner, PileType.Hand);
    }
}