using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class GhostShield : HexaghostCardModel, IHasAfterlifeEffect
{
    public GhostShield() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithAfterlife();
        WithBlock(7, 3);
        WithPower<BlurPower>(1);
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AfterlifeEffect(ctx, cardPlay);
        await CommonActions.ApplySelf<BlurPower>(ctx, this);
    }
}