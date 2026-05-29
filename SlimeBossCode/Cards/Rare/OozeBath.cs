using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;
using Downfall.DownfallCode.Artists;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class OozeBath : SlimeBossCardModel
{
    public OozeBath() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<OozeBathPower>(6, 3);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<OozeBathPower>(ctx, this, cardPlay);
    }
}