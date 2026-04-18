using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class IllTakeThat : CollectorCardModel
{
    public IllTakeThat() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithVar("IllTakeThat", 10, 3);
        WithDamage(10, 3);
        WithTip(StaticHoverTip.Block);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
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