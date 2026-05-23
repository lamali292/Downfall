using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Removed;

[Obsolete]
[Pool(typeof(TokenCardPool))]
public class Batch() : AutomatonCardModel(0, CardType.Skill, CardRarity.Token, TargetType.Self);