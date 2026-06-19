using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class Schlurp : SlimeBossCardModel
{
    public Schlurp() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithCards(1, 1);
        WithPower<GoopPower>(7);
    }

    protected override Artist Artist => Artist.Get<Freshbone>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<GoopPower>(ctx, this, cardPlay);
        await DownfallCardCmd.GiveCards<Lick>(Owner, PileType.Hand, DynamicVars.Cards.BaseValue);
    }
}