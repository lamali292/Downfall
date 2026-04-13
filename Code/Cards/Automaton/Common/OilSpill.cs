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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Automaton.Common;

[Pool(typeof(AutomatonCardPool))]
public class OilSpill() : AutomatonCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), IEncodable,
    ICompilableError
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new PowerVar<PoisonPower>(4)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode),
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromKeyword(DownfallKeywords.Compile),
        HoverTipFactory.FromCard<Slimed>()
    ];


    public async Task OnCompileError(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext,
        bool forGameplay)
    {
        await DownfallCardCmd.Insert(ModelDb.Card<Slimed>(), Owner);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
        await PowerCmd.Apply<PoisonPower>(cardPlay.Target, DynamicVars["PoisonPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars["PoisonPower"].UpgradeValueBy(1);
    }
}