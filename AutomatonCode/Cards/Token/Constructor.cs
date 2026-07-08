using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Constructor : AutomatonCardModel, IEncodable
{
    public Constructor() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithBlock(5, 2);
        WithVars(new BlockVar("ExtraBlock", 5, ValueProp.Move).WithUpgrade(2));
    }
    
    public IEnumerable<Encodable> Encodings => [new BlockEncode()];

    public void ApplyEncode(FunctionCard function, FunctionPosition position)
    {
        if (position == FunctionPosition.Start)
        {
            function.DynamicVars.Block.BaseValue += DynamicVars["ExtraBlock"].BaseValue;
        }
    }
}