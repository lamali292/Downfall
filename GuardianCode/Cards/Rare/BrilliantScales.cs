using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class BrilliantScales : GuardianCardModel, IGemSocketCard
{
    public BrilliantScales() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<BrilliantScalesPower>(1, false);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    public override bool CanBeGeneratedInCombat => false;

    public int GemSlots => IsUpgraded ? 3 : 2;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await CommonActions.ApplySelf<BrilliantScalesPower>(ctx, this);
        power?.SetCard(this);
    }
}