using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class WreathOfVictory : ChampCardModel
{
    public WreathOfVictory() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithKeyword(CardKeyword.Exhaust);
        this.WithTip<VigorPower>();
        this.WithTip<CounterPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var vigor = Owner.Creature.GetPowerAmount<VigorPower>();
        var counter = Owner.Creature.GetPowerAmount<CounterPower>();
        await CommonActions.ApplySelf<VigorPower>(ctx, this, vigor);
        await CommonActions.ApplySelf<CounterPower>(ctx, this, counter);
    }
}