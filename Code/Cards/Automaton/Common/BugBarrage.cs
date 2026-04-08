using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Downfall.Code.Cards.Automaton.Common;

[Pool(typeof(AutomatonCardPool))]
public class BugBarrage : AutomatonCardModel
{
    public BugBarrage() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithTip(DownfallKeyword.Cycle);
        WithTip(DownfallKeyword.Status);
        WithTip(typeof(Wound));
    }

    protected override async Task PlayEffect(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var wound1 = CombatState?.CreateCard<Wound>(Owner);
        var wound2 = CombatState?.CreateCard<Wound>(Owner);
        if (wound1 == null || wound2 == null) return;
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardsToCombat(
                [wound1, wound2],
                PileType.Hand,
                true
            )
        );
        var hand = PileType.Hand.GetPile(Owner);
        var statuses = hand.Cards
            .Where(c => c.Type == CardType.Status)
            .ToList(); // snapshot before we modify
        await CardPileCmd.Add(statuses, PileType.Discard);
        await CardPileCmd.Draw(choiceContext, statuses.Count, Owner);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(statuses.Count)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}