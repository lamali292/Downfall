using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class ExoticFormPower : SneckoPowerModel, IHasSecondAmount
{
    private readonly HashSet<CardPoolModel> _uniqueColorsThisTurn = [];

    public string GetSecondAmount()
    {
        return _uniqueColorsThisTurn.Count.ToString();
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return Task.CompletedTask;
        if (_uniqueColorsThisTurn.Add(cardPlay.Card.Pool))
            InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
    

    public override async Task BeforeFlush(PlayerChoiceContext ctx, Player player)
    {
        if (player.Creature != Owner) return;

        var uniqueColorCount = _uniqueColorsThisTurn.Count;
        if (uniqueColorCount > 0)
        {
            await PowerCmd.Apply<StrengthPower>(ctx, Owner, Amount * uniqueColorCount, 
                Owner, null);
        }
        _uniqueColorsThisTurn.Clear();
        InvokeDisplayAmountChanged();
    }
}