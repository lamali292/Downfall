using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class LivingBomb : HexaghostCardModel
{
    public LivingBomb() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<SoulBurnPower>(4, 3);
        this.WithPower<LivingBombPower>(1, false);
    }

    protected override Artist Artist => Artist.Get<Magerblutooth>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
        await CommonActions.Apply<LivingBombPower>(ctx, this, cardPlay);
    }
}