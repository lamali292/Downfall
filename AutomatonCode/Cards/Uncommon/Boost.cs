using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Boost : AutomatonCardModel
{
    public Boost() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithEncode<BlockEncode>();
        WithCompile<StrengthCompile>();
        WithBlock(6);
        WithPower<StrengthPower>(2, 1);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();


}