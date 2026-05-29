using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Spellbinder : AwakenedCardModel
{
    public Spellbinder() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<SpellbinderPower>(1, false);
        this.WithConjure();
        WithCostUpgradeBy(-1);
    }
    protected override Artist Artist => Artist.Get<Eudaimonia>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SpellbinderPower>(ctx, this);
    }
}