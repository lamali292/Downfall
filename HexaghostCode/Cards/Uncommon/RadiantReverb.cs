using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class RadiantReverb : HexaghostCardModel
{
    public RadiantReverb() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(14, 4);
        this.WithPower<TemporaryIntensityPower>(3, 2, false);
        this.WithTip<IntensityPower>();
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.ApplySelf<TemporaryIntensityPower>(ctx, this);
    }
}