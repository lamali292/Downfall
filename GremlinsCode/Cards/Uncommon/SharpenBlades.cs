using BaseLib.Utils;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gremlins.GremlinsCode.Cards.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class SharpenBlades : GremlinsCardModel
{
    public SharpenBlades() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(0, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
        Owner.Hand
            .Where(e => e.Type == CardType.Attack).ToList()
            .ForEach(e => e.EnergyCost.SetThisTurn(0));
    }
}