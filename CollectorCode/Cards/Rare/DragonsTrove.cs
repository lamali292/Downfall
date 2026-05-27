using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class DragonsTrove : CollectorCardModel, IHasPyre
{
    public DragonsTrove() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithCards(2);
        WithVar("Reserve", 1, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    public CardModel? PyredCard { get; set; }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CollectorCmd.DrawCollected(ctx, Owner, DynamicVars.Cards.IntValue);
        CardResourceRegistry.Get<CollectorEnergy>()?.Gain(Owner, DynamicVars["Reserve"].IntValue);
    }
}