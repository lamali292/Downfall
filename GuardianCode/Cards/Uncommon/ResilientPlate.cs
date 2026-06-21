using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class ResilientPlate : GuardianCardModel
{
    public ResilientPlate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        this.WithBrace(8);
        this.WithPolish(2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Brace(ctx, this);
        await GuardianCmd.Polish(ctx, this);
    }
}