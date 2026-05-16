using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Philosophize : AutomatonCardModel,
    IEncodable, ICompilableError
{
    public Philosophize() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<StrengthPower>(1);
        WithPower<StrengthPower>("EnemyStrength", 2, -1);
        WithTip(AutomatonTip.Encode);
        WithTip(AutomatonTip.Compile);
    }

    public async Task OnCompileError(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext, bool forGameplay)
    {
        var state = Owner.Creature.CombatState;
        ArgumentNullException.ThrowIfNull(state);
        await PowerCmd.Apply<StrengthPower>(ctx, state.HittableEnemies, DynamicVars["EnemyStrength"].BaseValue,
            Owner.Creature,
            this);
    }


    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }
}