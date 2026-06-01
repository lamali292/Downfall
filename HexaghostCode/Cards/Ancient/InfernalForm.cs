using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Ancient;

[Pool(typeof(HexaghostCardPool))]
public class InfernalForm : HexaghostCardModel
{
    public InfernalForm() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithPower<InfernalFormPower>(2, 1);
        WithKeyword(CardKeyword.Innate);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<InfernalFormPower>(ctx, this);
    }
}