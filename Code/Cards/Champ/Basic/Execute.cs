using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Basic;

[Pool(typeof(ChampCardPool))]
public class Execute : ChampCardModel
{
    public Execute() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(6);
    }

    // TODO: Implement

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}