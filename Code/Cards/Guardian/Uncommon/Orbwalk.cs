using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Orbwalk : GuardianCardModel, ITickCard
{
    public Orbwalk() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<StrengthPower>(3);
        WithKeyword(DownfallKeywords.Volatile, UpgradeType.Remove);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(this);
    }

    public async Task OnTick(PlayerChoiceContext ctx)
    {
        await CommonActions.ApplySelf<StrengthPower>(this, 1);
    }
}