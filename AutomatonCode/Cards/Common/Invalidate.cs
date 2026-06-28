using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Invalidate() : AutomatonCardModel(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy),
    IEncodable<InvalidateEncode>
{
    protected override Artist Artist => Artist.Get<Opal>();
    
}