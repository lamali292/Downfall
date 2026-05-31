using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class IllTakeThat : CollectorCardModel
{
    public IllTakeThat() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithVar("IllTakeThat", 10, 3);
        WithDamage(10, 3);
        WithTip(StaticHoverTip.Block);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var stolenBlock = Math.Min(cardPlay.Target.Block, DynamicVars["IllTakeThat"].IntValue);
        if (stolenBlock > 0)
        {
            await CreatureCmd.LoseBlock(cardPlay.Target, stolenBlock);
            await CreatureCmd.GainBlock(Owner.Creature, stolenBlock, ValueProp.Move | ValueProp.Unpowered, cardPlay);
        }

        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}