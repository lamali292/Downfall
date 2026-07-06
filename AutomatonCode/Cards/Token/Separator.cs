using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Separator : AutomatonCardModel, IEncodable<SeparatorEncode>
{
    public Separator() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithVars(new DamageVar("ExtraDamage", 6, ValueProp.Move).WithUpgrade(2));
    }
}