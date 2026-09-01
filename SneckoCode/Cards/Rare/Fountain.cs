using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Fountain : SneckoCardModel
{
    public Fountain() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip<VenomPower>();
        WithPower<FountainPower>(3, 2, false);
        WithTip(SneckoKeywords.Overflow);
    }
    
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<FountainPower>(ctx, this);
    }
}