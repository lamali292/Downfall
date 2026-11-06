using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class TornadoPunch : ChampCardModel
{
    public TornadoPunch() : base(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(10, 2);
        WithBlock(5, 2);
        WithVar("LastHitCount", 0);
        WithDefensiveCombo();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var result = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var hitCount = result.Results.SelectMany(r => r).Count(x => x.TotalDamage > 0);
        await ChampCmd.DefensiveCombo(cardPlay, async () =>
        {
            for (var i = 0; i < hitCount; i++)
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        });
    }
}