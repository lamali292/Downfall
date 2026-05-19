using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Basic;

[Pool(typeof(AutomatonCardPool))]
public class Replicate : AutomatonCardModel, IEncodable
{
    public Replicate() : base(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
    }

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
        var result = await CardPileCmd.AddGeneratedCardToCombat(copiedCard, PileType.Discard, Owner);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result, 0.4f);
    }
}