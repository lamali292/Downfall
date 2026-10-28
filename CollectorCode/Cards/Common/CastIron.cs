using Collector.CollectorCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Collector.CollectorCode.Powers;
namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class CastIron : CollectorCardModel
{
    public CastIron() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithCostUpgradeBy(-1);
        WithCardTip<Ember>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var pyred = await PyreCmd.Pyre(ctx, this);
        if (pyred == null) return;
        var embers = pyred.EnergyCost.GetAmountToSpend();
        if (embers > 0)
            await DownfallCardCmd.GiveCards<Ember>(Owner, PileType.Draw, embers, CardPilePosition.Random);
    }
}
