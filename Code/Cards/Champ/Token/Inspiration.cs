using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Core;
using Downfall.Code.Core.Champ;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Champ.Token;

[Pool(typeof(TokenCardPool))]
public class Inspiration : ChampCardModel
{
    public Inspiration() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithKeywords(CardKeyword.Retain);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var stance = Owner.ChampStance();
        switch (stance)
        {
            case BerserkerChampStance:
                await ChampCmd.EnterDefensiveStance(ctx, Owner);
                break;
            case DefensiveChampStance:
                await ChampCmd.EnterBerserkerStance(ctx, Owner);
                break;
            default:
            {
                var rng = CombatState!.RunState.Rng.CombatCardSelection;
                if (rng.NextBool())
                    await ChampCmd.EnterDefensiveStance(ctx, Owner);
                else
                    await ChampCmd.EnterBerserkerStance(ctx, Owner);
                break;
            }
        }
    }


    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}