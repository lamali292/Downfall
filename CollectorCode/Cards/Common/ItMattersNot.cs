using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class ItMattersNot : CollectorCardModel
{
    public ItMattersNot() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithBlock(9, 3);
        WithTip<VulnerablePower>();
        WithTip<WeakPower>();
        WithTip<MiasmaPower>();
        WithVar("ItMattersNot", 1);
    }

    private static decimal Calc(CardModel card, Creature? creature)
    {
        if (card is ItMattersNot usesPyredCards)
        {
            return usesPyredCards.PyredCard?.EnergyCost.GetAmountToSpend() ?? 0;
        }
        return 0;
    }

    protected override Artist Artist => Artist.Get<Opal>();

    private CardModel? PyredCard { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CommonActions.CardBlock(this, cardPlay);
        
    }
}