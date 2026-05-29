using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class MakeshiftBlade : SneckoCardModel, IHasGift
{
    public MakeshiftBlade() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        this.WithGift(new Gift
        {
            IsDebuff = true
        });
        WithDamage(9, 4);
        WithCards(3);
        WithVar("Debuffs", 3);
    }

    public Gift? Gift { get; set; }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (cardPlay.Target?.Powers.Count(e => e is { Type: PowerType.Debuff, Amount: > 0 }) >=
            DynamicVars["Debuffs"].IntValue) await CommonActions.Draw(this, ctx);
    }
}