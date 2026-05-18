using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class RockSlide : GuardianCardModel
{
    public RockSlide() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(30, 12);
        WithTip(GuardianKeyword.Gem);
    }

    public override int GemSlots => 3;

    public override void AfterCreated()
    {
        var rng = Owner.RunState.Rng.Niche;

        AddGem(GuardianModelDb.AllGems
            .Where(e => e.Rarity == CardRarity.Common)
            .TakeRandom(1, rng)
            .First()
            .ToMutable());
        AddGem(GuardianModelDb.AllGems
            .Where(e => e.Rarity == CardRarity.Uncommon)
            .TakeRandom(1, rng)
            .First()
            .ToMutable());
        AddGem(GuardianModelDb.AllGems
            .Where(e => e.Rarity == CardRarity.Rare)
            .TakeRandom(1, rng)
            .First()
            .ToMutable());
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}