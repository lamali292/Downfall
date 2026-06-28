using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class CultistStrike : AutomatonCardModel, IEncodable<CultistStrikeEncode>
{
    public CultistStrike() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithVar("Increase", 1, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var increase = DynamicVars["Increase"].IntValue;
        (EncodeModifier.On(this) as CultistStrikeEncode)?.Buff(increase);
        if (DeckVersion is { } deck)
            (EncodeModifier.On(deck) as CultistStrikeEncode)?.Buff(increase);

        return Task.CompletedTask;
    }
}