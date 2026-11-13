using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Multiplayer;

[Pool(typeof(HexaghostCardPool))]
public class BroilingFlames : HexaghostCardModel
{
    public BroilingFlames() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<BroilingFlamesPower>(6, 2, false);
        WithKeywords(CardKeyword.Exhaust);
        WithTip<SoulBurnPower>();
    }

    protected override Artist Artist => Artist.Get<Inmo>();


    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<BroilingFlamesPower>(ctx, this, cardPlay);
    }
}