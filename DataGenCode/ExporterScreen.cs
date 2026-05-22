using DataGen.DataGenCode.Exporter;
using Godot;

namespace DataGen.DataGenCode;

public partial class ExporterScreen : Control
{
    private ColorRect _bg = new() { Color = Colors.DarkGreen with { A = 0.97f } };
    private Button _closeButton = new() { Text = "Close" };

    private bool _closing;
    private Button _deleteButton = new() { Text = "Delete Folder" };
    private Button _exportButton = new() { Text = "Export!" };

    private ExportBatch? _exporter;
    private CheckBox _exportImages = new() { Text = "Export images?", ButtonPressed = true };
    private Button _openButton = new() { Text = "Open Folder" };
    private Label _statusLabel = new() { Text = "", HorizontalAlignment = HorizontalAlignment.Center };
    private Label _testLabel = new() { Text = "StS2 Exporter", HorizontalAlignment = HorizontalAlignment.Center };
    private CheckBox _texDump = new() { Text = "Include full texture dump?", ButtonPressed = false };
    private VBoxContainer _vBox = new();


    public override void _Ready()
    {
        base._Ready();
        Modulate = Modulate with { A = 0f };
        SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_bg);
        _bg.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_vBox);
        _vBox.SetAnchorsPreset(LayoutPreset.Center);
        _vBox.AddChild(_closeButton);
        _vBox.AddChild(_testLabel);
        _vBox.AddChild(_exportImages);
        _vBox.AddChild(_texDump);
        _vBox.AddChild(_openButton);
        _vBox.AddChild(_deleteButton);
        _vBox.AddChild(_exportButton);
        _vBox.AddChild(_statusLabel);
        _closeButton.Connect(BaseButton.SignalName.Pressed, Callable.From(Close));
        _openButton.Connect(BaseButton.SignalName.Pressed, Callable.From(OpenDir));
        _deleteButton.Connect(BaseButton.SignalName.Pressed, Callable.From(DeleteDir));
        _exportButton.Connect(BaseButton.SignalName.Pressed, Callable.From(Export));
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 1f, 0.25f);
    }

    public override void _Process(double delta)
    {
        Size = GetParent<Control>().Size;
        _vBox.Position = Size / 2f - _vBox.Size / 2f;
        UpdateExportingProgress();
    }

    private void UpdateExportingProgress()
    {
        if (_exporter == null) return;
        if (_exporter.ImagesExported >= _exporter.NumImagesToExport)
        {
            _deleteButton.Disabled = false;
            _exportButton.Disabled = false;
            _closeButton.Disabled = false;
            ShowStatus($"Done! Exported {_exporter.NumImagesToExport} images in total.");
            _exporter = null;
            return;
        }

        ShowStatus($"{_exporter.ImagesExported}/{_exporter.NumImagesToExport} images exported");
    }

    private void Close()
    {
        if (_closing) return;
        _closing = true;
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0f, 0.25f);
        tween.TweenCallback(Callable.From(QueueFree));
    }

    private void OpenDir()
    {
        if (ExportBatch.OpenDir()) return;
        ShowError("Folder does not exist! Start exporting first.");
    }

    private void DeleteDir()
    {
        ExportBatch.DeleteDir();
        ClearStatus();
    }

    private void Export()
    {
        if (ExportBatch.DirExists())
        {
            ShowError("Export directory already exists! You must delete it first.");
            return;
        }

        _deleteButton.Disabled = true;
        _exportButton.Disabled = true;
        _closeButton.Disabled = true;
        _exporter = new ExportBatch();
        _exporter.Run(new ExportConfig
        {
            ExportImages = _exportImages.ButtonPressed,
            DoTexDump = _texDump.ButtonPressed
        });
        if (_exporter.NumImagesToExport != 0) return;
        _exporter = null;
        _deleteButton.Disabled = false;
        _exportButton.Disabled = false;
        _closeButton.Disabled = false;
        ShowStatus("Done!");
    }

    private void ClearStatus()
    {
        ShowStatus("");
    }

    private void ShowError(string text)
    {
        ShowStatus(text, Colors.Red);
    }

    private void ShowStatus(string text, Color? color = null)
    {
        _statusLabel.Text = text;
        _statusLabel.Modulate = color ?? Colors.Cyan;
    }
}