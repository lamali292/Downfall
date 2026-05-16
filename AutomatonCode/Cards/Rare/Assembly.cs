using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Assembly : AutomatonCardModel
{
    public Assembly() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithVar("Scry", 5, 3);
        WithTip(AutomatonTip.Encode);
        WithTip(DownfallTip.Scry);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var result = await ScryCmd.Execute(ctx, Owner, DynamicVars["Scry"].IntValue);
        foreach (var card in result.Discarded.Where(e => e is IEncodable { AutoEncode: true }))
        {
            if (card is not IEncodable encodable) continue;
            await encodable.Encode(ctx, cardPlay);
        }
    }
}