using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Smother : SlimeBossCardModel
{
    public Smother() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithBlock(5, 1);
        WithPower<WeakPower>(1, 1);
        WithTip<Slimed>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        await DownfallCardCmd.GiveCard<Slimed>(Owner, PileType.Hand);
    }
}
