using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class BranchBlock : AutomatonCardModel, IEncodable
{
    public BranchBlock() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithBlock(6, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public IEnumerable<Encodable> Encodings => [new BlockEncode()];
    
    public override bool CanBeGeneratedByModifiers => false;
    public override bool CanBeGeneratedInCombat => false;
}