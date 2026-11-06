using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class KillingSpree : ChampCardModel
{
    public KillingSpree() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<KillingSpreePower>(1, false);
        WithTip(ChampKeyword.TriggerSkillBonus);
        // WithTip(ChampTip.Stance);
        WithVar("Skill", 3, 2);
    }

    protected override Artist Artist => Artist.Get<Fulgur>();
    protected override bool IsBetaArt => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<KillingSpreePower>(ctx, this);
        for (var i = 0; i < DynamicVars["Skill"].IntValue; i++) await Owner.ChampStance.SkillBonus(ctx);
    }
}