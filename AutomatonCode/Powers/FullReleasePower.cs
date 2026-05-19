using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class FullReleasePower : AutomatonPowerModel
{
    private IReadOnlyList<AutomatonCardModel> _sourceCards = [];

    public FullReleasePower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithVars(new EffectsDynamicVar());
    }

    public override bool ShouldReceiveCombatHooks => true;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;


    public void SetSourceCards(IReadOnlyList<AutomatonCardModel> sourceCards)
    {
        _sourceCards = sourceCards;
    }


    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (Owner.Player != player || Owner.CombatState == null) return;
        var resourceInfo = new ResourceInfo
        {
            EnergySpent = 0,
            EnergyValue = 0,
            StarsSpent = 0,
            StarValue = 0
        };
        for (var i = 0; i < _sourceCards.Count; i++)
            if (_sourceCards[i] is IEncodable encodable)
            {
                var target = Owner.Player.RunState.Rng.CombatTargets.NextItem(Owner.CombatState.HittableEnemies);
                await encodable.PlayEncodableEffect(choiceContext, new CardPlay
                {
                    Card = _sourceCards[i],
                    Target = target,
                    ResultPile = PileType.None,
                    Resources = resourceInfo,
                    IsAutoPlay = true,
                    PlayIndex = 0,
                    PlayCount = 1
                }, new EncodeContext(true, i));
            }
    }

    private class EffectsDynamicVar : DynamicVar
    {
        private FullReleasePower? _power;

        public EffectsDynamicVar() : base("effects", 0)
        {
        }

        public override void SetOwner(AbstractModel model)
        {
            base.SetOwner(model);
            _power = model as FullReleasePower;
        }

        public override string ToString()
        {
            if (_power == null) return "";
            var lines = _power._sourceCards
                .Select((c, i) => (c as IEncodable)?.GetEncodeLocString(new EncodeContext(true, i)))
                .Where(loc => loc != null)
                .Select(loc => loc!.GetFormattedText())
                .ToList();
            return lines.Count > 0 ? string.Join("\n", lines) : "";
        }
    }
}
