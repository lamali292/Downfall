using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Frontload : AutomatonCardModel
{
    public Frontload() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithEncode<BlockEncode>();
        WithCompile<RetainCompile>();
        WithTip(CardKeyword.Retain);
        WithBlock(8, 3);
    }

    public override bool GainsBlock => true;

    protected override Artist Artist => Artist.Get<Opal>();

}