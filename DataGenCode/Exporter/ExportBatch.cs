using System.Text.Encodings.Web;
using System.Text.Json;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes;
using FileAccess = Godot.FileAccess;

namespace DataGen.DataGenCode.Exporter;

public class ExportBatch
{
    private const string BaseDir = "./export";
    private static readonly string TexDumpDir = BaseDir.PathJoin("texDump");
    private readonly ItemList _items = new();

    private readonly Dictionary<string, ModExport> _mods = [];

    public int ImagesExported;
    public int NumImagesToExport;

    public void Run(ExportConfig config)
    {
        DirAccess.MakeDirRecursiveAbsolute(BaseDir);
        FindMods();
        FindItems();
        AssignItemsToMods();
        DiscardBasegame();
        ExportMods();
        ExportAllData();
        if (config.DoTexDump)
            DumpTextures();
        if (config.ExportImages)
            ExportImages(config);
        Finish();
    }

    private void FindMods()
    {
        foreach (var mod in ModManager.Mods)
        {
            ModExport export = new(mod);
            if (export.Id == null) continue;
            _mods.Add(export.Id, export);
        }
    }

    private void FindItems()
    {
        _items.FindAll();
    }
    private void AssignItemsToMods()
    {
        foreach (var item in _items.All())
        {
            try
            {
                var mod = _mods.Values.FirstOrDefault(m => m.Assembly != null && m.Assembly == item.Assembly);
                if (mod == null)
                {
                    GD.Print($"No mod found for item {item.GetType().Name} assembly {item.Assembly?.GetName().Name}");
                    continue;
                }
                mod.AddItem(item);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    private void DiscardBasegame()
    {
        _items.RemoveIf(i => i.Mod?.IsBasegame ?? true);
        _mods.Remove("slay-the-spire-2");
    }
    private void ExportMods()
    {
        foreach (var (_, mod) in _mods.Where(m => m.Value.Items.All().Any()))
            mod.Export(BaseDir);
    }

    private void ExportAllData()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        Write("items.json", _items);
        Write("cards.json", _items.Cards);
        Write("relics.json", _items.Relics);
        Write("potions.json", _items.Potions);
        return;

        void Write(string name, object data)
        {
            var file = FileAccess.Open($"{BaseDir}/{name}", FileAccess.ModeFlags.Write);
            file.StoreString(JsonSerializer.Serialize(data, options));
            file.Close();
        }
    }


    private void ExportImages(ExportConfig config)
    {
        NGame._window.Size = new Vector2I(1920, 1080);
        foreach (var item in _items.All())
            if (item is IImageExport imageExport)
                foreach (var request in imageExport.ExportImg(config))
                {
                    NumImagesToExport++;
                    ViewportManager.RequestDraw(request).ContinueWith(task =>
                    {
                        var img = task.Result;

                        var path = $"{BaseDir}/{item.Mod?.Id}/{request.Path}.png";
                        DirAccess.MakeDirRecursiveAbsolute(path[..path.LastIndexOf('/')]);
                        img.SavePng(path);
                        ImagesExported++;
                    });
                }
    }

    public static bool OpenDir()
    {
        if (!DirAccess.DirExistsAbsolute(BaseDir)) return false;
        OS.ShellOpen(BaseDir);
        return true;
    }

    public static bool DirExists()
    {
        return DirAccess.DirExistsAbsolute(BaseDir);
    }

    public static void DeleteDir()
    {
        DeleteRecursive(BaseDir);

        static void DeleteRecursive(string dir)
        {
            foreach (var d in DirAccess.GetDirectoriesAt(dir))
                DeleteRecursive(dir.PathJoin(d));
            foreach (var f in DirAccess.GetFilesAt(dir))
                DirAccess.RemoveAbsolute(dir.PathJoin(f));
            DirAccess.RemoveAbsolute(dir);
        }
    }

    private void Finish()
    {
        GD.Print("Export finished!");
    }

    private static void DumpTextures()
    {
        DumpDir("res://");
        AtlasManager.LoadAllAtlases();
        foreach (var atlasName in AtlasManager._knownAtlases)
        {
            var atlasPath = AtlasResourceLoader._atlasBasePath.PathJoin(atlasName);
            var savePath = atlasPath.Replace("res:/", TexDumpDir);
            var atlas = AtlasManager._atlases[atlasName];
            foreach (var (spriteName, _) in atlas.SpriteMap)
            {
                var texture = AtlasManager.GetSprite(atlasName, spriteName);
                var path = savePath.PathJoin($"{spriteName}.png");
                DirAccess.MakeDirRecursiveAbsolute(path[..path.LastIndexOf('/')]);
                if (texture != null)
                    ViewportManager.RequestDraw(new ViewportManager.DrawRequest((Vector2I)texture.GetSize(),
                            action: drawer => drawer.DrawTexture(texture, Vector2.Zero)))
                        .ContinueWith(task => task.Result.SavePng(path));
            }
        }

        return;


        static void DumpDir(string path)
        {
            var exportPath = path.Replace("res:/", TexDumpDir);
            var files = ResourceLoader.ListDirectory(path);
            foreach (var file in files)
                if (file.EndsWith('/'))
                {
                    DumpDir(path.PathJoin(file));
                }
                else if (file.EndsWith(".png"))
                {
                    DirAccess.MakeDirRecursiveAbsolute(exportPath);
                    var filePath = path.PathJoin(file);
                    var resource = ResourceLoader.Load(filePath);
                    if (resource is Texture2D texture)
                        texture.GetImage().SavePng(exportPath.PathJoin(file));
                    else if (resource is Image image)
                        image.SavePng(exportPath.PathJoin(file));
                }
        }
    }
}