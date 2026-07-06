using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class AncestralGrounds : AwakenedCardModel
{
    public AncestralGrounds() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(12);
        WithEnergy(2, 1);
        this.WithPower<AncestralGroundsPower>(2, false);
        this.WithPower<AncestralGroundsUpgradedPower>(2, false);
        this.WithTip<Void>();
    }


    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        if (IsUpgraded)
            await CommonActions.ApplySelf<AncestralGroundsUpgradedPower>(ctx, this);
        else
            await CommonActions.ApplySelf<AncestralGroundsPower>(ctx, this);
    }
}