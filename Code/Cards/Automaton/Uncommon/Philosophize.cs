using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Philosophize() : AutomatonCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self),
    IEncodable, ICompilableError
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1),
        new PowerVar<StrengthPower>("EnemyStrength", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode),
        HoverTipFactory.FromKeyword(DownfallKeywords.Compile),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public async Task OnCompileError(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext, bool forGameplay)
    {
        var state = Owner.Creature.CombatState;
        ArgumentNullException.ThrowIfNull(state);
        await PowerCmd.Apply<StrengthPower>(state.Enemies, DynamicVars["EnemyStrength"].BaseValue, Owner.Creature,
            this);
    }


    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars.Strength.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["EnemyStrength"].UpgradeValueBy(-1);
    }
}