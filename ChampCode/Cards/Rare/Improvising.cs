using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class Improvising : ChampCardModel
{
    public Improvising() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<ImprovisingPower>(2, 1, false);
        WithTip(ChampTip.Stance);
        WithTip(ChampKeyword.TriggerSkillBonus);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ImprovisingPower>(ctx, this);
    }
}