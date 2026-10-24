using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Cards.Ancient;

namespace SlimeBoss.SlimeBossCode.Cards.Basic;

[Pool(typeof(SlimeBossCardPool))]
public class PressTheAttack : SlimeBossCardModel, ITranscendenceCard
{
    public PressTheAttack() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithCostUpgradeBy(-1);
        WithDamage(5);
        WithSlimeTip<BruiserSlime>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await SlimeBossCmd.Command<BruiserSlime>(ctx, Owner, 1, this);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<PileOn>();
    }
}
