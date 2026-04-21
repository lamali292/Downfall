using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Downfall;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class IronFortress : ChampCardModel
{
    public IronFortress() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<DexterityPower>(2);
        WithPower<MetallicizePower>(3, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DexterityPower>(this);
        await CommonActions.ApplySelf<MetallicizePower>(this);
    }
}