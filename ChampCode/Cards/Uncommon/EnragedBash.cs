using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class EnragedBash : ChampCardModel, IBerserkerComboCard
{
    public EnragedBash() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
        WithRepeat(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue)
            .Execute(ctx);
    }
    
    public Task BerserkerComboEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        DynamicVars.Repeat.UpgradeValueBy(1);
        return Task.CompletedTask;
    }
}