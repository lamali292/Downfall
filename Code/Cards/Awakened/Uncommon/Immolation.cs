using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Immolation : AwakenedCardModel, IOnDrained
{
    public Immolation() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(13);
        WithKeywords(CardKeyword.Retain);

    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    public Task OnDrained(Player player, int amount)
    {
        if (player == Owner)
        {
            EnergyCost.AddUntilPlayed(-amount);
        }
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}