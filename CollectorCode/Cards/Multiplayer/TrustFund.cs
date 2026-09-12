using BaseLib.Abstracts;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Multiplayer;

[Pool(typeof(CollectorCardPool))]
public class TrustFund : CollectorCardModel
{
    public TrustFund() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        WithReserve(1, 1);
        WithTip(StaticHoverTip.Block);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
     
        var targetCreature = cardPlay.Target;
        var targetPlayer = targetCreature?.Player;
        if (targetCreature == null || targetPlayer == null) return;
        var targetBlock = targetCreature.Block;
      
        var ownerCreature = Owner.Creature;
        var ownerBlock = ownerCreature.Block;

        await CompatibilityCreatureCmd.LoseBlock(ctx, targetCreature, targetBlock, ownerCreature);
        await CompatibilityCreatureCmd.LoseBlock(ctx, ownerCreature, ownerBlock, ownerCreature);
        
        await CreatureCmd.GainBlock(targetCreature, ownerBlock, BlockProps.cardUnpowered, cardPlay);
        await CreatureCmd.GainBlock(ownerCreature, targetBlock, BlockProps.cardUnpowered, cardPlay);

        await CollectorCmd.GainReserve(Owner, DynamicVars.Reserve.IntValue);
        await CollectorCmd.GainReserve(targetPlayer, DynamicVars.Reserve.IntValue);
    }
}