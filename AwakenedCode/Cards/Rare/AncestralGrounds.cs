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
        this.WithTip<Void>();
    }


    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        if (IsUpgraded)
            await CommonActions.ApplySelf<AncestralGroundsUpgradedPower>(ctx, this, 2);
        else
            await CommonActions.ApplySelf<AncestralGroundsPower>(ctx, this, 2);
    }
}