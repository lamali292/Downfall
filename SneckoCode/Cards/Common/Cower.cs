using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Cards.Token;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Cower : SneckoCardModel, IAfterCardMuddled
{
    public Cower() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(14, 4);
        WithPower<WeakPower>(1);
        WithTip(SneckoKeywords.Muddle);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    public async Task AfterCardMuddled(PlayerChoiceContext ctx, CardModel card, AbstractModel? source)
    {
        if (card != this) return;
        await CommonActions.ApplySelf<WeakPower>(ctx, this);
    }
}