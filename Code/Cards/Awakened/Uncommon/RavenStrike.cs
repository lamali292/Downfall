using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Displays;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class RavenStrike : AwakenedCardModel, IChantable
{
    public RavenStrike() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(15);
        WithTip(DownfallKeyword.Chant);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
    
    public async Task OnChant(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.AutoPlayFromDrawPile(ctx, Owner, 1);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
    }
}