using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.Code.Powers.Automaton;

public class FullReleasePower :AutomatonPowerModel
{
    private IReadOnlyList<AutomatonCardModel> _sourceCards = [];
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    public override bool ShouldReceiveCombatHooks => true;
    public override bool IsInstanced => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EffectsDynamicVar(this)];

    public void SetSourceCards(IReadOnlyList<AutomatonCardModel> sourceCards)
    {
        _sourceCards = sourceCards;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.CombatState == null) return;
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

    private class EffectsDynamicVar(FullReleasePower power) : DynamicVar("effects", 0)
    {
        public override string ToString()
        {
            var lines = power._sourceCards
                .Select((c, i) => (c as IEncodable)?.GetEncodeLocString(new EncodeContext(true, i)))
                .Where(loc => loc != null)
                .Select(loc => loc!.GetFormattedText())
                .ToList();
            return lines.Count > 0 ? string.Join("\n", lines) : "";
        }
    }
}