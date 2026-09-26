using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class HeavyTackle : SlimeBossCardModel
{
    public HeavyTackle() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTags(SlimeBossTag.Tackle);
        WithDamage(11, 3);
        WithTip<Slimed>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await DownfallCardCmd.GiveCard<Slimed>(Owner, PileType.Draw, CardPilePosition.Top);
    }
}
