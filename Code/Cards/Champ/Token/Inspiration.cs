using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Core;
using Downfall.Code.Core.Champ;
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
        var stance = ChampModel.GetStanceModel(Owner);
        switch (stance)
        {
            case BerserkerStance:
                await ChampModel.SetStance<DefensiveStance>(ctx, Owner);
                break;
            case DefensiveStance:
                await ChampModel.SetStance<BerserkerStance>(ctx, Owner);
                break;
            default:
            {
                var rng = CombatState!.RunState.Rng.CombatCardSelection;
                if (rng.NextBool())
                    await ChampModel.SetStance<DefensiveStance>(ctx, Owner);
                else
                    await ChampModel.SetStance<BerserkerStance>(ctx, Owner);
                break;
            }
        }
    }


    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}