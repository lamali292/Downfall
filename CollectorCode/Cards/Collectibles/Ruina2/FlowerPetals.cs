using BaseLib.Utils;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public class FlowerPetals : Ruina2Card
{
    public FlowerPetals() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, "ALRIUNE_ELITE")
    {
        WithPower<MiasmaPower>(6, 2);
        WithPower<WeakPower>(1, 1);
        WithPower<VulnerablePower>(1, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<MiasmaPower>(ctx, this, cardPlay);
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
    }
}