using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class OozeBath : SlimeBossCardModel
{
    public OozeBath() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        this.WithPower<OozeBathPower>(6, 3, false);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<OozeBathPower>(ctx, this, cardPlay);
    }
}