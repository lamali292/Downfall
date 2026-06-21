using Automaton.AutomatonCode.Cards.Uncommon;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class InfiniteLoopPower : AutomatonPowerModel, IAfterCompilingFunction
{
    private CardModel? _copy;

    public InfiniteLoopPower()
    {
        WithVars(new CardDynamicVar());
        WithTips(Tip);
    }

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public async Task AfterCompilingFunction(PlayerChoiceContext ctx, Player player, CardPileAddResult result)
    {
        if (_copy == null || player.Creature != Owner) return;
        await CardPileCmd.AddGeneratedCardToCombat(_copy, PileType.Hand, player);
        await PowerCmd.Remove(this);
    }

    private static IEnumerable<IHoverTip> Tip(PowerModel arg)
    {
        return arg is InfiniteLoopPower { _copy: not null } power ? [new CardHoverTip(power._copy)] : [];
    }

    public void SetCard(InfiniteLoop infiniteLoop)
    {
        _copy = infiniteLoop.CreateClone();
        _copy.EnergyCost.AfterCardPlayedCleanup();
        _copy.EnergyCost.EndOfTurnCleanup();
        _copy.DynamicVars.Damage.UpgradeValueBy(Amount);
        _copy.DynamicVars.FinalizeUpgrade();
    }

    private class CardDynamicVar() : DynamicVar("card", 0)
    {
        private InfiniteLoopPower? _power;

        public override void SetOwner(AbstractModel model)
        {
            base.SetOwner(model);
            _power = model as InfiniteLoopPower;
        }

        public override string ToString()
        {
            return _power?._copy == null ? "?" : _power._copy.Title;
        }
    }
}