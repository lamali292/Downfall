using BaseLib.Utils;
using Hermit.HermitCode.Cards.Basic;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public class HairTrigger: HermitCardModel
{
    public HairTrigger() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        this.WithPower<HairTriggerPower>(1, false);
        WithTip(CardKeyword.Exhaust);
        WithTip(HermitKeywords.DeadOn);
        this.WithCardTip<StrikeHermit>((e, _) => e.AddKeyword(CardKeyword.Exhaust));
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<HairTriggerPower>(ctx, this);
    }
}