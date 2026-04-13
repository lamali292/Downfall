using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Spellbinder : AwakenedCardModel
{
    public Spellbinder() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<SpellbinderPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.ApplySelf<SpellbinderPower>(this);
    }

  

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}