using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Frontload : AutomatonCardModel, IEncodable
{
    public Frontload() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithTip(CardKeyword.Retain);
        WithBlock(8, 3);
    }

    public IEnumerable<Encodable> Encodings => [new BlockEncode()];
    
    public override bool GainsBlock => true;

    protected override Artist Artist => Artist.Get<Opal>();
    
    public void ApplyEncode(FunctionCard function, FunctionPosition position)
    {
        function.AddKeyword(CardKeyword.Retain);
    }
}