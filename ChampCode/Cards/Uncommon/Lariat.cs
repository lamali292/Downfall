using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class Lariat : ChampCardModel
{
    public Lariat() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(5, 2);
        WithTip(ChampKeyword.TriggerSkillBonus);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = ResolveEnergyXValue();
        for (var i = 0; i < amount; i++) await CommonActions.CardBlock(this, cardPlay);
        for (var i = 0; i < amount; i++) await Owner.ChampStance().SkillBonus(ctx);
    }
}