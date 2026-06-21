using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class ShieldSpikes : GuardianCardModel
{
    public ShieldSpikes() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(12, 4);
        WithPower<ThornsPower>(3, 1);
        this.WithBrace(8);
        WithTip(GuardianTip.DefensiveMode);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        if (GuardianCmd.IsInMode<GuardianDefensiveMode>(Owner)) await CommonActions.ApplySelf<ThornsPower>(ctx, this);
        await GuardianCmd.Brace(ctx, this);
    }
}