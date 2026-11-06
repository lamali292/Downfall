using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class ThermalTransfer : HexaghostCardModel
{
    public ThermalTransfer() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 2);
        WithBlock(6, 2);
        WithTip<SoulBurnPower>();
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var hasSoulburn = cardPlay.Target.HasPower<SoulBurnPower>();
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!hasSoulburn) return;
        await CommonActions.CardBlock(this, cardPlay);
    }
}