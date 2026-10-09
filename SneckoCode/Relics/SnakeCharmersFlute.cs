using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Relics;

[Obsolete]
[Pool(typeof(SneckoRelicPool))]
public class SnakeCharmersFlute : SneckoRelicModel, IMaxMuddleCost
{
    public SnakeCharmersFlute() : base(RelicRarity.Ancient, false)
    {
        WithTip(SneckoKeywords.Muddle);
    }

    public int ModifyMaxMuddleCost(CardModel card, int i)
    {
        return card.Owner == Owner ? i - 1 : i;
    }

    public Task AfterModifyingMaxMuddleCost(PlayerChoiceContext choiceContext, CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }
}