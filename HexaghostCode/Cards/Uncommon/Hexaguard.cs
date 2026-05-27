using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class Hexaguard : HexaghostCardModel, IHasAfterlifeEffect
{
    public Hexaguard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithAfterlife();
        WithBlock(6, 3);
        WithCards(2);
    }

    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
        await AfterlifeEffect(ctx, cardPlay);
    }
}