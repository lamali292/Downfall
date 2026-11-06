using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
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
        WithTip<VigorPower>();
        WithTip<CounterPower>();
    }

    protected override Artist Artist => Artist.Get<Fulgur>();
    protected override bool IsBetaArt => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var vigor = Owner.Creature.GetPowerAmount<VigorPower>();
        var counter = Owner.Creature.GetPowerAmount<CounterPower>();
        await CommonActions.ApplySelf<VigorPower>(ctx, this, vigor);
        await CommonActions.ApplySelf<CounterPower>(ctx, this, counter);
    }
}