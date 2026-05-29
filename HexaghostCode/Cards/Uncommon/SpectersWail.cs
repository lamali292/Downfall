using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class SpectersWail : HexaghostCardModel, IHasAfterlifeEffect
{
    public SpectersWail() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        this.WithAfterlife();
        WithDamage(4, 2);
    }


    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithAttackerFx("vfx/vfx_spooky_scream").Execute(ctx);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 2).WithAttackerFx("vfx/vfx_spooky_scream").Execute(ctx);
    }
}