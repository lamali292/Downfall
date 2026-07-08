using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Boost : AutomatonCardModel, IEncodable, ICompilable
{
    public Boost() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(5);
        WithPower<StrengthPower>(2, 1);
    }
    
    public IEnumerable<Encodable> Encodings => [new BlockEncode()];

    protected override Artist Artist => Artist.Get<AlexMdle>();
    

    public Task OnCompile(PlayerChoiceContext ctx)
    {
        return CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }
}