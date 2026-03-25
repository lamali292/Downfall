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
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class InfiniteLoop() : AutomatonCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy),
    IEncodable, ICompilable
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move),
        new IntVar("Increase", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode),
        HoverTipFactory.FromKeyword(DownfallKeywords.Compile)
    ];


    public async Task OnCompile(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext, bool forGameplay)
    {
        if (!forGameplay) return;

        var copy = CreateClone();
        copy.DynamicVars.Damage.UpgradeValueBy(DynamicVars["Increase"].IntValue);
        copy.DynamicVars.FinalizeUpgrade();

        var result = await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, true);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Increase"].UpgradeValueBy(2);
    }
}