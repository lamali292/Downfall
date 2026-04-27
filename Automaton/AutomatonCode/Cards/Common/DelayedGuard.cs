using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class DelayedGuard : AutomatonCardModel, IEncodable
{
    public DelayedGuard() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<BlockNextTurnPower>(7, 3);
        WithTip(AutomatonTip.Encode);
        WithTip(StaticHoverTip.Block);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CommonActions.ApplySelf<BlockNextTurnPower>(ctx, this);
    }
}