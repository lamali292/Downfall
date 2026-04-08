using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Automaton.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Virus : AutomatonCardModel
{
    public Virus() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(4, 2);
        WithKeywords(CardKeyword.Exhaust);
        WithTip(new TooltipSource(card =>
        {
            var beam = ModelDb.GetById<MinorBeam>(ModelDb.Card<MinorBeam>().Id).ToMutable();
            if (card.IsUpgraded) beam.UpgradeInternal();
            return HoverTipFactory.FromCard(beam);
        }));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);


        var hand = PileType.Hand.GetPile(Owner).Cards.ToList();
        await CardCmd.Discard(ctx, hand);

        var beams = hand.Select(_ =>
        {
            var beam = CombatState!.CreateCard<MinorBeam>(Owner);
            if (IsUpgraded) beam.UpgradeInternal();
            return beam;
        }).ToList();

        await CardPileCmd.AddGeneratedCardsToCombat(beams, PileType.Hand, true);
    }
}