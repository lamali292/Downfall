using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class VoidArmor : CollectorCardModel
{
    public VoidArmor() : base(1, CardType.Skill, CardRarity.Uncommon, CustomTarget.Everyone)
    {
        WithBlock(10, 3);
        WithPower<BlurPower>(1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        //await CommonActions.Apply<StrengthPower>(ctx,cardPlay.Target, this, 1);
        if (CombatState == null) return;
        foreach (var creature in CombatState.Creatures)
            await CreatureCmd.GainBlock(creature, DynamicVars.Block, cardPlay);

        await PowerCmd.Apply<BlurPower>(ctx, CombatState.Creatures, DynamicVars.Power<BlurPower>().IntValue, Owner.Creature,
            this);
    }
}