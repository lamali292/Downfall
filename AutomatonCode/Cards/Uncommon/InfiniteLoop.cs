using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class InfiniteLoop : AutomatonCardModel,
    IEncodable, ICompilable
{
    public InfiniteLoop() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(6);
        WithVar("Increase", 2, 2);
    }
    
    public IEnumerable<Encodable> Encodings => [new DamageEncode()];

    protected override Artist Artist => Artist.Get<Opal>();

    public async Task OnCompile(PlayerChoiceContext context)
    {
        var copy = CreateClone();
        copy.EnergyCost.AfterCardPlayedCleanup();
        copy.EnergyCost.EndOfTurnCleanup();
        copy.DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].BaseValue);
        copy.DynamicVars.FinalizeUpgrade();
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, Owner);
    }
}