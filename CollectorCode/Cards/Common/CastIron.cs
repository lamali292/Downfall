using BaseLib.Abstracts;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class CastIron : CollectorCardModel
{
    public CastIron() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithCards(2);
        WithKeyword(CardKeyword.Exhaust);
        WithUpgradeChangingCardTip<Burn, Ember>();
        WithCalculatedVar("Kindle", 0, 3, Calc);
        WithTip(CollectorTip.Kindle);
    }

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return card.Owner.Hand.Count(e => e.Type == CardType.Status);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)//Todo: Finish this later
    {
        if (IsUpgraded)
        {
            await DownfallCardCmd.GiveCards<Ember>(Owner, PileType.Hand, DynamicVars.Cards.IntValue);
        }
        else
        {
            await DownfallCardCmd.GiveCards<Burn>(Owner, PileType.Hand, DynamicVars.Cards.IntValue);
        }
        var repeat = (int)((CalculatedVar)DynamicVars["Kindle"]).Calculate(null);
        await CollectorCmd.Kindle(ctx,Owner, repeat, this);
    }

}