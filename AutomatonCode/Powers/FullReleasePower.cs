using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Abstracts;
using Downfall.DownfallCode.Commands;
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
    private IReadOnlyList<CardModifier> SourceCards = [];

    public FullReleasePower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithVars(new EffectsDynamicVar());
    }

    public override bool ShouldReceiveCombatHooks => true;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;


    public void SetSourceCards(IReadOnlyList<CardModifier> sourceCards)
    {
        SourceCards = sourceCards;
    }


    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (Owner.Player != player || Owner.CombatState == null) return;
        var resourceInfo = new ResourceInfo
        {
            EnergySpent = 0,
            EnergyValue = 0,
            StarsSpent = 0,
            StarValue = 0
        };
        
        foreach (var card in SourceCards)
        {
            var target = Owner.Player.RunState.Rng.CombatTargets.NextItem(Owner.CombatState.HittableEnemies);
            var cardPlay = new CardPlay
            {
                Card = card.Owner,
                Target = target,
                ResultPile = PileType.None,
                Resources = resourceInfo,
                IsAutoPlay = true,
                PlayIndex = 0,
                PlayCount = 1
            };
            
            await card.OnPlay(choiceContext, cardPlay);
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
            var i = 0;
            var lines = new List<string>();
            foreach (var card in _power.SourceCards)
            {
                if (card is IEncodable encodable)
                {
                    var text = encodable.GetEncodeLocString(new EncodeContext(true, i))?.GetFormattedText();
                    if (text == null) continue;
                    lines.Add(text);
                }

                i++;
            }

            return lines.Count > 0 ? string.Join("\n", lines) : "";
        }
    }
}