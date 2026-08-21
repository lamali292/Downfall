using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class OilSpill : AutomatonCardModel, IEncodable, ICompilable
{
    public OilSpill() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(4, 1);
        WithPower<PoisonPower>(4, 1);
        WithTip(AutomatonTip.Stash);
        this.WithTip<Error>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public Task OnCompile(PlayerChoiceContext ctx)
    {
        return StashCmd.Stash<Error>(ctx, Owner);
    }

    public IEnumerable<Encodable> Encodings => [new DamageEncode(), new PoisonEncode()];
}