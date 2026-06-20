using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class ChargedBarrage : HexaghostCardModel
{
    public ChargedBarrage() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<SoulBurnPower>(6, 2);
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var count = HexaghostCmd.GetIgnitedCount(Owner);
        for (var i = 0; i < count; i++)
            await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
    }
}