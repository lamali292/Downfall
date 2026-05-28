using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class AdrenalArmor : ChampCardModel
{
    public AdrenalArmor() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 2);
        this.WithPower<TemporaryStrengthUpPower>(2, 1, false);
        this.WithTip<StrengthPower>();
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<TemporaryStrengthUpPower>(ctx, this);
    }
}