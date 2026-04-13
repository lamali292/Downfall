using BaseLib.Utils;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Character.Automaton;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Backtrace() : AutomatonCardModel(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IEncodable,
    ICompilableError
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move)];
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode),
        HoverTipFactory.FromKeyword(DownfallKeywords.Compile),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public Task OnCompileError(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext, bool forGameplay)
    {
        card.AddKeyword(CardKeyword.Exhaust);
        return Task.CompletedTask;
    }


    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }


    public override void ApplyToFunctionPreview(FunctionCard card)
    {
        if (SuppressCompileError) return;
        card.AddKeyword(CardKeyword.Exhaust);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}