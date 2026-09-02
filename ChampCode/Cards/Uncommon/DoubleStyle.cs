using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class DoubleStyle : ChampCardModel
{
    public DoubleStyle() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<DefensiveStylePower>(1, 1, false);
        WithPower<BerserkerStylePower>(1, 1, false);
        WithTip<VigorPower>();
        WithTip<CounterPower>();
        WithTip(ChampKeyword.TriggerSkillBonus);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DefensiveStylePower>(ctx, this);
        await CommonActions.ApplySelf<BerserkerStylePower>(ctx, this);
    }
}