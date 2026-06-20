using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Hermit.HermitCode.Cards.Common;

public sealed class Manifest : HermitCardModel
{
    private const int BlockAmount = 16;
    private const int UpgradedBlockAmount = 20;

    public Manifest() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(16, 4);
        this.WithTip<Decay>();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);
        await DownfallCardCmd.GiveCard<Decay>(Owner, PileType.Hand);
    }
}