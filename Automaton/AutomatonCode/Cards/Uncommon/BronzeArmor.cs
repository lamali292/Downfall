using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class BronzeArmor : AutomatonCardModel, IEncodable,
    ICompilableError
{
    public BronzeArmor() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<ArtifactPower>(1);
        WithVar("EnemyBlock", 12, -4);
        WithTip(AutomatonTip.Encode);
        WithTip(AutomatonTip.Compile);
    }

    public async Task OnCompileError(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext,
        bool forGameplay)
    {
        var state = Owner.Creature.CombatState;
        ArgumentNullException.ThrowIfNull(state);
        foreach (var combatStateEnemy in state.Enemies)
            await CreatureCmd.GainBlock(combatStateEnemy, DynamicVars["EnemyBlock"].BaseValue, ValueProp.Unpowered,
                cardPlay);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CommonActions.ApplySelf<ArtifactPower>(ctx, this);
    }
}