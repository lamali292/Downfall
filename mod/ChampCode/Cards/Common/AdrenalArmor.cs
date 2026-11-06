using BaseLib.Abstracts;
using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class AdrenalArmor : ChampCardModel
{
    public AdrenalArmor() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6, 2);
        WithPower<AdrenalArmorPower>(3, 1, false);
        WithTip<StrengthPower>();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<AdrenalArmorPower>(ctx, this);
    }
}

public class AdrenalArmorPower : CustomTemporaryPowerModelWrapper<AdrenalArmor, StrengthPower>;