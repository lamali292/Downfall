using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Displays;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class BronzeArmor() : AutomatonCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IEncodable,
    ICompilableError
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("EnemyBlock", 12),
        new PowerVar<ArtifactPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DownfallKeyword.Encode.ToHoverTip(),
        HoverTipFactory.FromPower<ArtifactPower>(),
        DownfallKeyword.Compile.ToHoverTip()
    ];

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
        await PowerCmd.Apply<ArtifactPower>(Owner.Creature, DynamicVars["ArtifactPower"].BaseValue, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["EnemyBlock"].UpgradeValueBy(-4);
    }
}