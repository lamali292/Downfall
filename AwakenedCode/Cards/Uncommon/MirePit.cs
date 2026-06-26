using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using Awakened.AwakenedCode.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class MirePit : AwakenedCardModel
{
    public MirePit() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        this.WithPower<MirePitPower>(6, 2, false);
        this.WithTip<StrengthPower>();
        this.WithDrained(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        foreach (var combatStateEnemy in CombatState.Enemies)
            await CommonActions.Apply<MirePitPower>(ctx, combatStateEnemy, this);

        await CommonActions.ApplySelf<DrainedPower>(ctx, this);
    }
}

public class MirePitPower : CustomTemporaryPowerModelWrapper<MirePit, StrengthPower>
{
    protected override bool InvertInternalPowerAmount => true;
}