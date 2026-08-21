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
    public SphericShield() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithBrace(10, 3);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();
    
    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = EnergyCost.GetResolved();
        for (var i = 0; i < x; i++)
        {
            await GuardianCmd.Brace(ctx, this);
        }
    }
}