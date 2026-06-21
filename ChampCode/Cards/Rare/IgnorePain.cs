using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class IgnorePain : ChampCardModel
{
    public IgnorePain() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithKeywords(CardKeyword.Exhaust);
        this.WithFinisher();
        this.WithPower<IgnorePainPower>(1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<IgnorePainPower>(ctx, this);
    }
}