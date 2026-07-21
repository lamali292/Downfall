using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Interfaces;
using Downfall.DownfallCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Random;

namespace Downfall.DownfallCode.Cards;

[Pool(typeof(TokenCardPool))]
#pragma warning disable
public class CharacterCard() : ConstructedCardModel(-1, CardType.Skill, CardRarity.Token, TargetType.Self),
    IModfyCardDescription, ICustomPortrait
#pragma warning restore
{
    private ImageTexture? _cachedTexture;
    internal CharacterModel? CharacterModel;
    public CardModel? RandomCommonCard;
    public CardModel? RandomRareCard;
    public CardModel? RandomUncommonCard;

    protected override bool IsPlayable => false;

    public override string Title => CharacterModel == null
        ? "???"
        : new LocString("characters", CharacterModel.CharacterSelectTitle)
            .GetFormattedText();

    public Texture2D? GetPortraitTexture()
    {
        if (_cachedTexture != null) return _cachedTexture;

        _cachedTexture = PortraitCompositor.SliceHorizontally(
            [RandomCommonCard?.Portrait, RandomUncommonCard?.Portrait, RandomRareCard?.Portrait]);

        return _cachedTexture;
    }

    public LocString ModifyDescription(LocString oldLocString)
    {
        return CharacterModel == null ? oldLocString : new LocString("characters", CharacterModel.CharacterSelectDesc);
    }

    public static CharacterCard Create(CharacterModel characterModel)
    {
        var a = ModelDb.Card<CharacterCard>().ToMutable();
        if (a is not CharacterCard characterCard) throw new Exception("CharacterCard model is not a CharacterCard");
        characterCard.CharacterModel = characterModel;
        characterCard._pool = characterModel.CardPool;
        characterCard.RandomCommonCard = Rng.Chaotic.NextItem(characterModel.CardPool.AllCards
            .Where(e => e.Rarity == CardRarity.Common));
        characterCard.RandomUncommonCard = Rng.Chaotic.NextItem(characterModel.CardPool.AllCards
            .Where(e => e.Rarity == CardRarity.Uncommon));
        characterCard.RandomRareCard = Rng.Chaotic.NextItem(characterModel.CardPool.AllCards
            .Where(e => e.Rarity == CardRarity.Rare));
        NCard.FindOnTable(characterCard)?.Reload();
        return characterCard;
    }
}