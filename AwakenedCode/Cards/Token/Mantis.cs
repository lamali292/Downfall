using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Mantis : AwakenedCardModel
{
    public Mantis() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithPower<StrengthPower>(2, 1);
        this.WithTip<PlumeJab>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        await DownfallCardCmd.GiveCard<PlumeJab>(Owner, PileType.Hand, animationTime: 0.1f);
    }
}