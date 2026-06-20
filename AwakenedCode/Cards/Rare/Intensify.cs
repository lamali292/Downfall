using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Intensify : AwakenedCardModel
{
    public Intensify() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<IntensifyPower>(1, false);
        this.WithPower<BurnoutPower>(1, false);
        this.WithConjure();
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await AwakenedCmd.Conjure(Owner);
        await CommonActions.ApplySelf<IntensifyPower>(ctx, this);
        await CommonActions.ApplySelf<BurnoutPower>(ctx, this);
    }
}