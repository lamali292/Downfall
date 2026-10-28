using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class JadedJabs : CollectorCardModel
{
    public JadedJabs() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithTip(CollectorTip.Pyred);
        WithDamage(11, 3);
        WithBlock(3, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var pyred = await PyreCmd.Pyre(ctx, this);
        if (pyred == null) return;
        var cost = pyred.EnergyCost.GetAmountToSpend();
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (cost > 0)
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue * cost, DynamicVars.Block.Props, cardPlay);
    }
}
