using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class RockSlide : GuardianCardModel, IGemSocketCard
{
    public RockSlide() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(30, 12);
        WithTip(GuardianKeyword.Gem);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    public int GemSlots => 3;

    public override void AfterCreated()
    {
        var rng = Owner.RunState.Rng.Niche;

        if (this is not IGemSocketCard socketCard) return;
        foreach (var rarity in new[] { CardRarity.Common, CardRarity.Uncommon, CardRarity.Rare })
        {
            var gem = rng.NextItem(GuardianModelDb.AllGems.Where(e => e.Rarity == rarity))?.ToMutable();
            if (gem != null) socketCard.AddGem(gem);
        }
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}