using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;
[Pool(typeof(ChampCardPool))]
public class FlurryOfStrikes : ChampCardModel, IOnChampStanceChange
{
    public FlurryOfStrikes() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithTags(CardTag.Strike);
        WithTip(ChampTip.Stance);
    }


    public async Task OnChampStanceChange(PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance,
        ChampStanceModel newStance)
    {
        if (newStance.Owner != Owner || Pile?.Type != PileType.Discard || (newStance is ChampNoStance)) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}