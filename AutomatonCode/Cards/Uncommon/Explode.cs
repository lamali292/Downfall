using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Explode : AutomatonCardModel, IEncodable, ICompilable
{
    public Explode() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithCards(1);
        WithPower<SoulBurnPower>(15, 5);
        WithTip<Burn>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public Task OnCompile(PlayerChoiceContext context)
    {
        return DownfallCardCmd.GiveCards<Burn>(Owner, PileType.Draw, DynamicVars.Cards.BaseValue,
            CardPilePosition.Random);
    }

    public IEnumerable<Encodable> Encodings => [new SoulburnEncode()];
}