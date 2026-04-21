using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Core.Champ;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class FlurryOfStrikes : ChampCardModel, IOnChampStanceChange
{
    public FlurryOfStrikes() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithTags(CardTag.Strike);
    }


    public async Task OnChampStanceChange(PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance,
        ChampStanceModel newStance)
    {
        if (newStance.Owner != Owner && Pile?.Type != PileType.Discard) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}