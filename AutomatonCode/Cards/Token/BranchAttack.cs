using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class BranchAttack : AutomatonCardModel, IEncodable
{
    public BranchAttack() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(7, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public IEnumerable<Encodable> Encodings => [new DamageEncode()];
    
    public override bool CanBeGeneratedByModifiers => false;
    public override bool CanBeGeneratedInCombat => false;
}