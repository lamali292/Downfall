using BaseLib.Abstracts;
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
        WithPower<RadiantReverbPower>(4, 2, false);
        WithTip<IntensityPower>();
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.ApplySelf<RadiantReverbPower>(ctx, this);
    }
}

public class RadiantReverbPower : CustomTemporaryPowerModelWrapper<RadiantReverb, IntensityPower>;