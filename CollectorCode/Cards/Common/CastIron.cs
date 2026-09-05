using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class CastIron : CollectorCardModel
{
    public CastIron() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCards(2);
        WithKindle(3);
        WithUpgradeChangingCardTip<Burn, Ember>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            await DownfallCardCmd.GiveCards<Ember>(Owner, PileType.Hand, DynamicVars.Cards.IntValue);
        }
        else
        {
            await DownfallCardCmd.GiveCards<Burn>(Owner, PileType.Hand, DynamicVars.Cards.IntValue);
        }

        var handContents = Owner.Hand;
        foreach (var handContent in handContents)
        {
            if (handContent.Type == CardType.Status)
            {
                await CollectorCmd.SummonTorchhead(ctx, Owner, DynamicVars.Summon.IntValue, this);
            }
        }
    }

}