using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Mantis : AwakenedCardModel
{
    public Mantis() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithPower<StrengthPower>(2, 1);
        WithTip(typeof(PlumeJab));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(this);
        await DownfallCardCmd.GiveCard<PlumeJab>(Owner, PileType.Hand, animationTime: 0.1f);
    }
}