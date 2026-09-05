using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class RotwoodKindling : CollectorCardModel
{
    public RotwoodKindling() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithPower<MiasmaPower>(6, 1);
        WithKindle(4, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this || CombatState == null) return;
        await CommonActions.Apply<MiasmaPower>(ctx, CombatState.HittableEnemies, this);
        await CollectorCmd.Kindle(ctx, this);
    }
}