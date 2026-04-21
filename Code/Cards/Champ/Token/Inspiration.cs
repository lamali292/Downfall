using BaseLib.Utils;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Champ.Token;

[Pool(typeof(TokenCardPool))]
public class Inspiration : ChampCardModel
{
    public Inspiration() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithKeywords(CardKeyword.Retain);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.EnterDifferentStance(ctx, Owner);
    }


    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}