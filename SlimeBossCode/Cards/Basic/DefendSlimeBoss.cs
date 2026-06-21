using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Basic;

[Pool(typeof(SlimeBossCardPool))]
public class DefendSlimeBoss : SlimeBossCardModel
{
    public DefendSlimeBoss() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithTags(CardTag.Defend);
        WithBlock(5, 3);
    }

    protected override Artist Artist => Artist.Get<HalfGoblinHankins>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}