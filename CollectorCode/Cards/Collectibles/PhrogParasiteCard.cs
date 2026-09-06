using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;

namespace Collector.CollectorCode.Cards.Collectibles;

public class PhrogParasiteCard : Collectible<PhrogParasiteElite>
{
    public PhrogParasiteCard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f)
    {
        WithBlock(16, 5);
        WithCardTip<Infection>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await DownfallCardCmd.GiveCard<Infection>(Owner, PileType.Hand);
    }
}