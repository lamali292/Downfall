using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
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
    public Mesmerize() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        this.WithPower<MesmerizePower>(3, 3, false);
        this.WithTip<StrengthPower>();
        WithKeyword(CardKeyword.Exhaust);
        this.WithMuddle(1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CommonActions.Apply<MesmerizePower>(ctx, this, cardPlay);
        await SneckoCmd.MuddleHandCards(ctx, this);
    }
}

public class MesmerizePower : TemporaryDebuffPowerWrapper<Mesmerize, StrengthPower>;