using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Automaton.Basic;

[Pool(typeof(AutomatonCardPool))]
public sealed class Replicate()
    : AutomatonCardModel(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy), IEncodable
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DownfallKeyword.Encode.ToHoverTip()
    ];


    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }


    public async Task OnEncode(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState!;
        var copiedCard = combatState.CloneCard(cardPlay.Card);
        var result = await CardPileCmd.AddGeneratedCardToCombat(copiedCard, PileType.Discard, true);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result, 0.4f);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}