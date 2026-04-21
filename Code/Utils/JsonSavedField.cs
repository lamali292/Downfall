using System.Text.Json;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Utils;

/// <summary>
///     Static wrapper around SavedSpireField for automatic JSON serialization.
///     Must be used as a static readonly field.
/// </summary>
public static class JsonSavedField
{
    public static JsonSavedField<TModel, TValue> Create<TModel, TValue>(string key)
        where TModel : AbstractModel
    {
        return new JsonSavedField<TModel, TValue>(key);
    }
}

public class JsonSavedField<TModel, TValue> where TModel : AbstractModel
{
    private readonly SavedSpireField<TModel, string?> _field;

    internal JsonSavedField(string key)
    {
        _field = new SavedSpireField<TModel, string?>(() => null, key);
    }

    public TValue? Get(TModel model)
    {
        var json = _field.Get(model);
        return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<TValue>(json);
    }

    public void Set(TModel model, TValue? value)
    {
        var json = value == null ? null : JsonSerializer.Serialize(value);
        _field.Set(model, json);
    }
}