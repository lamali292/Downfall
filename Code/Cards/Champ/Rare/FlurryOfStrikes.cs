using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Core.Champ;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class FlurryOfStrikes : ChampCardModel, IOnStanceChange
{
    public FlurryOfStrikes() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }


    protected override void OnUpgrade()
    {
    }

    public async Task OnStanceChange(PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance, ChampStanceModel newStance)
    {
      
    }
}