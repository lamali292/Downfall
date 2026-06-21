using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class SphericShield : GuardianCardModel
{
    public SphericShield() : base(4, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithBrace(40);
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Brace(ctx, this);
    }
}