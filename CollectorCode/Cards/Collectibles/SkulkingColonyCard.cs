using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;

public class SkulkingColonyCard : Collectible<SkulkingColonyElite>
{
    public SkulkingColonyCard() : base(5, CardType.Status, CardRarity.Uncommon, TargetType.Self, 0.3f)
	
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CollectorKeyword.Flicker);
        // todo: we do plating now. idk if thats good conversion of numbers.
        WithPower<PlatingPower>(5, 3);
    }

    protected override async Task OnFlicker(PlayerChoiceContext ctx)
    {
        await CommonActions.ApplySelf<PlatingPower>(ctx, this);
    }
}