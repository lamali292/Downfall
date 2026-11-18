using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class SpectersWail : HexaghostCardModel, IHasAfterlifeEffect
{
    public SpectersWail() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithAfterlife();
        WithDamage(4, 2);
        WithHitCount(2);
    }


    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, bool wasExhausted,
        bool causedByEthereal)
    {
        await HexaghostCmd.AfterlifeAttack(this, cardPlay).WithAttackerFx("vfx/vfx_spooky_scream").Execute(ctx);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.HitCountCompat.IntValue).WithAttackerFx("vfx/vfx_spooky_scream").Execute(ctx);
    }
}