using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Assembly : AutomatonCardModel
{
    public Assembly() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithVar("Scry", 5);
        WithTip(DownfallKeyword.Encode);
        WithTip(DownfallKeyword.Scry);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var result = await ScryCmd.Execute(ctx, Owner, DynamicVars["Scry"].IntValue);
        foreach (var card in result.Discarded.Where(e => e is IEncodable { AutoEncode: true }))
        {
            if (card is not IEncodable encodable) continue;
            await encodable.Encode(ctx, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Scry"].UpgradeValueBy(3);
    }
}