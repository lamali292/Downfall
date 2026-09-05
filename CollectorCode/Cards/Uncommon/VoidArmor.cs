using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class VoidArmor : CollectorCardModel
{
    public VoidArmor() : base(1, CardType.Skill, CardRarity.Uncommon, DownfallTargetType.MeAndEnemies)
    {
        WithBlock(10, 3);
        WithPower<BlurPower>(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        //await CommonActions.Apply<StrengthPower>(ctx,cardPlay.Target, this, 1);
        await CommonActions.CardBlock(this, cardPlay);
        if (CombatState == null) return;
        foreach (var creature in CombatState.HittableEnemies)
            await CreatureCmd.GainBlock(creature, DynamicVars.Block, cardPlay);
        await CommonActions.Apply<BlurPower>(ctx, this, cardPlay);
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("Multiplayer", DownfallCmd.IsMultiplayer);
    }
}