using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class Outflare : HexaghostCardModel
{
    public Outflare() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 3);
        this.WithPower<OutflarePower>(1, 1, false);
        this.WithTip<IntensityPower>();
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<OutflarePower>(ctx, this);
    }
}

public class OutflarePower : CustomTemporaryPowerModelWrapper<Outflare, IntensityPower>;