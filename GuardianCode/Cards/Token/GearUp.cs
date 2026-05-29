using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Guardian.GuardianCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class GearUp : GuardianCardModel
{
    public GearUp() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
        this.WithBrace(10, 5);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Brace(ctx, this);
    }
}