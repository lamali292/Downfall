using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class FlamesFromBeyond : HexaghostCardModel, IHasAfterlifeEffect
{
    public FlamesFromBeyond() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        this.WithAfterlife();
        WithPower<SoulBurnPower>(10, 3);
    }


    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AfterlifeEffect(ctx, cardPlay);
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
    }
}