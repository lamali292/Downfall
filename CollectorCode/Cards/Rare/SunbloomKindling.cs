using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class SunbloomKindling : CollectorCardModel
{
    public SunbloomKindling() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Unplayable);
        WithPower<StrengthPower>(2, 3);
        WithCards(2);
        this.WithTip<Ember>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        await DownfallCardCmd.GiveCards<Ember>(Owner, PileType.Hand, DynamicVars.Cards.IntValue);
    }
}