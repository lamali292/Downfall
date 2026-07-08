using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Philosophize : AutomatonCardModel, IEncodable, ICompilable
{
    public Philosophize() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar("EnemyStrength", 2, -1);
        WithPower<StrengthPower>(1);
        this.WithTip<StrengthPower>();
    }

    public IEnumerable<Encodable> Encodings => [new StrengthEncode()];
    
    protected override Artist Artist => Artist.Get<Opal>();

    public Task OnCompile(PlayerChoiceContext ctx)
    {
        if (Owner.Creature.CombatState == null) return Task.CompletedTask;
        var enemies = Owner.Creature.CombatState.HittableEnemies;
        return PowerCmd.Apply<StrengthPower>(ctx, enemies, DynamicVars["EnemyStrength"].BaseValue,
            Owner.Creature, this);
    }
}