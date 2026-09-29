using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class BranchAttack : AutomatonCardModel
{
    public BranchAttack() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithEncode<DamageEncode>();
        WithDamage(7, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    
    public override bool CanBeGeneratedByModifiers => false;
    public override bool CanBeGeneratedInCombat => false;
}