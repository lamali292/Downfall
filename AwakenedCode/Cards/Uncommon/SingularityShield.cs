using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class SingularityShield : AwakenedCardModel
{
    public SingularityShield() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(8, 2);
        this.WithDrained(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<BlockNextTurnPower>(ctx, this, DynamicVars.Block.BaseValue);
        await CommonActions.ApplySelf<DrainedPower>(ctx, this);
    }
}