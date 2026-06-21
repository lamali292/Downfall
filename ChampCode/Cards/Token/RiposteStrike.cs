using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(TokenCardPool))]
public class RiposteStrike : ChampCardModel
{
    public RiposteStrike() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(0);
        WithKeywords(CardKeyword.Ethereal);
        WithKeywords(CardKeyword.Exhaust);
        WithTags(CardTag.Strike);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(ctx);
    }
}