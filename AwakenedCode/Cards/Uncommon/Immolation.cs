using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Events;
using BaseLib.Utils;
using Downfall.DownfallCode.Extensions.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Immolation : AwakenedCardModel, IOnDrained
{
    public Immolation() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(13, 4);
        WithKeywords(CardKeyword.Retain);
        WithTip(AwakenedTip.Drained.WithVars(new EnergyVar(1)));
    }

    public Task OnDrained(PlayerChoiceContext ctx, Player player, int amount)
    {
        if (player == Owner) EnergyCost.AddUntilPlayed(-amount);
        return Task.CompletedTask;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}