using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class DazingPulse : AutomatonCardModel, IEncodable, ICompilable
{
    public DazingPulse() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithBlock(7, 2);
        WithDamage(7, 2);
        WithCards(2);
        this.WithTip<Dazed>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public Task OnCompile(PlayerChoiceContext context)
    {
        return DownfallCardCmd.GiveCards<Dazed>(Owner, PileType.Draw, DynamicVars.Cards.BaseValue,
            CardPilePosition.Random);
    }

    public IEnumerable<Encodable> Encodings => [new BlockEncode(), new DamageEncode()];
}