using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Philosophize : AutomatonCardModel,
    IEncodable, ICompilable
{
    public Philosophize() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<StrengthPower>(1, 1);
        WithPower<StrengthPower>("EnemyStrength", 1);
    }

 
    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }
    
    public async Task OnCompile(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext, bool forGameplay)
    {
        var state = Owner.Creature.CombatState;
        if (state == null) return;
        await PowerCmd.Apply<StrengthPower>(ctx, state.HittableEnemies, DynamicVars["EnemyStrength"].BaseValue,
            Owner.Creature,
            this);
    }


}