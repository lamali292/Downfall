using BaseLib.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Ancient;

[Pool(typeof(SlimeBossCardPool))]
public class AncientOrobas : SlimeBossCardModel
{
    public AncientOrobas() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
        WithCards(1);
        WithKeywords(BaseLibKeywords.Purge, CardKeyword.Ethereal, CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
    }
}