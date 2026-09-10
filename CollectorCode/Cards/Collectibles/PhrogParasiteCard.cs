using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;
namespace Collector.CollectorCode.Cards.Collectibles;

public class PhrogParasiteCard : Collectible<PhrogParasiteElite>
{
    public PhrogParasiteCard() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0.4f)
    {
        WithBlock(20, 4);//Keep in mind manifest is a common.
        WithCardTip<Infection>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await DownfallCardCmd.GiveCard<Infection>(Owner, PileType.Hand);
    }
}