using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
[Obsolete]
public class NibbleAndLick()
    : SlimeBossCardModel(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, false, false)
{
    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();
}