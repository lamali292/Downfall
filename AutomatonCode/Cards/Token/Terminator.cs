using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Extensions;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Piles;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Terminator : AutomatonCardModel, IEncodable
{
    public Terminator() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithTip(StaticHoverTip.ReplayStatic);
    }
    
    public void ApplyEncode(FunctionCard function, FunctionPosition position)
    {
        if (position == FunctionPosition.End)
        {
            function.BaseReplayCount += 1;
        }
    }

    public IEnumerable<Encodable> Encodings => [];

}