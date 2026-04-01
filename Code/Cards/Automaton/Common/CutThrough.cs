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

namespace Downfall.Code.Cards.Automaton.Common;

[Pool(typeof(AutomatonCardPool))]
public class CutThrough : AutomatonCardModel, ICompilable,
    IEncodable
{
    public CutThrough() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(5);
        WithVar("Scry", 2);
        WithTip(DownfallKeyword.Encode);
        WithTip(DownfallKeyword.Compile);
        WithTip(DownfallKeyword.Scry);
    }

    // Compile bonus — draw 1 card
    public async Task OnCompile(PlayerChoiceContext ctx, FunctionCard function, CardPlay cardPlay,
        CompileContext compileContext,
        bool forGameplay)
    {
        if (!forGameplay) return;
        await CardPileCmd.Draw(ctx, Owner);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        await ScryCmd.Execute(ctx, Owner, 2);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["Scry"].UpgradeValueBy(1);
    }
}