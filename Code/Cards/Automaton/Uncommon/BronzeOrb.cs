using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class BronzeOrb : AutomatonCardModel
{
    public BronzeOrb() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 4);
        WithKeywords(CardKeyword.Innate);
        WithKeywords(CardKeyword.Exhaust);
        WithTip(DownfallKeyword.Encode);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);

        var eligibleCards = PileType.Draw.GetPile(Owner).Cards
            .Where(c => c is IEncodable)
            .ToList();
        var randomCard = eligibleCards.Count > 0
            ? eligibleCards[Random.Shared.Next(eligibleCards.Count)]
            : null;

        if (randomCard is not IEncodable encodable) return;
        await encodable.Encode(ctx, cardPlay);
    }
}