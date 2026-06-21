using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Multiplayer;

[Pool(typeof(ChampCardPool))]
public class ShowOff : ChampCardModel
{
    public ShowOff() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithCards(3, 1);
        WithTip(ChampKeyword.TriggerSkillBonus);
        WithTip(ChampTip.Stance);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Owner.ChampStance().SkillBonus(ctx);
        if (cardPlay.Target?.Player == null) return;
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.IntValue, cardPlay.Target.Player);
    }
}