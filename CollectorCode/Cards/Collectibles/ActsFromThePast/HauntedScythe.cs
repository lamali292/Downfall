using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class HauntedScythe : ActsFromThePastCard
{
    public HauntedScythe() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self, "NEMESIS_ELITE")
    {
        WithCostUpgradeBy(-1);
        WithPower<IntangiblePower>(1);
        WithCards(5);
        WithUpgradeChangingCardTip<Burn, Ember>();
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<IntangiblePower>(ctx, this);
        if (IsUpgraded)
        {
            await DownfallCardCmd.GiveCards<Ember>(Owner, PileType.Draw, DynamicVars.Cards.IntValue, CardPilePosition.Random);
        }
        else
        {
            await DownfallCardCmd.GiveCards<Burn>(Owner, PileType.Draw, DynamicVars.Cards.IntValue, CardPilePosition.Random);
        }
       
    }
}