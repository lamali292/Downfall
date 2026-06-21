using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Mesmerize : SneckoCardModel
{
    public Mesmerize() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar("StrengthLoss", 2, 1);
        WithKeyword(CardKeyword.Exhaust);
        this.WithMuddle(1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await PowerCmd.Apply<StrengthPower>(ctx, CombatState.HittableEnemies,
            -DynamicVars["StrengthLoss"].BaseValue, Owner.Creature, this);
        await SneckoCmd.MuddleHandCards(ctx, this);
    }
}