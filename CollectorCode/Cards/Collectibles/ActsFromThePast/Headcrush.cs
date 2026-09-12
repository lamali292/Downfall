using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

[Pool(typeof(TokenCardPool))]
public class Headcrush : CollectorCardModel
{
    public Headcrush() : base(4, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, IsModLoaded("ActsFromThePast"), IsModLoaded("ActsFromThePast"))
    {
        WithDamage(70, 30);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    private static bool IsModLoaded(string modId)
    {
        return ModManager.GetLoadedMods().Any(m => m.manifest?.id == modId);
    }
}