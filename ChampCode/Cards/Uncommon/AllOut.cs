using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class AllOut : ChampCardModel
{
    public AllOut() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithRepeat(2, 1);
        WithFinisher();
        // WithTip(ChampTip.Stance);
    }

    public override FinisherDescriptor Finisher =>
        new(KeepsStance: true, RepeatCount: () => DynamicVars.Repeat.IntValue);
}