using System;
using Godot;

public partial class DialogManager : FileDialog
{

  private static DialogManager Instance;
  private static bool IsBusy = false;

  public override void _Ready()
  {
    Instance = this;
  }

  public static bool RequestFile(Action<string> callback, string[] fileTypes, string defaultPath) =>
    RequestFileInternal(callback, fileTypes, defaultPath, mode: FileModeEnum.OpenFile);

  public static bool RequestSaveFile(Action<string> callback, string[] fileTypes, string defaultPath) =>
    RequestFileInternal(callback, fileTypes, defaultPath, mode: FileModeEnum.SaveFile);

  private static bool RequestFileInternal(Action<string> callback, string[] fileTypes, string defaultPath, FileModeEnum mode)
  {
    if (IsBusy)
      return false;

    Instance.FileMode = mode;
    Instance.Filters = fileTypes;
    Instance.CurrentPath = defaultPath;

    void ForwardSingleFile(string path)
    {
      callback?.Invoke(path);
      Unsubscribe();
    }

    void Unsubscribe()
    {
      Instance.FileSelected -= ForwardSingleFile;
      Instance.Canceled -= Unsubscribe;
      IsBusy = false;
    }

    IsBusy = true;
    Instance.FileSelected += ForwardSingleFile;
    Instance.Canceled += Unsubscribe;
    Instance.PopupCentered();
    return true;
  }

  public static bool RequestDirectory(Action<string> callback, string defaultPath)
  {
    if (IsBusy)
      return false;

    Instance.FileMode = FileModeEnum.OpenDir;
    Instance.Filters = [];
    Instance.CurrentPath = defaultPath;

    void ForwardDirectory(string path)
    {
      callback?.Invoke(path);
      Unsubscribe();
    }

    void Unsubscribe()
    {
      Instance.DirSelected -= ForwardDirectory;
      Instance.Canceled -= Unsubscribe;
      IsBusy = false;
    }

    IsBusy = true;
    Instance.DirSelected += ForwardDirectory;
    Instance.Canceled += Unsubscribe;
    Instance.PopupCentered();
    return true;
  }

  public static bool RequestFiles(Action<string[]> callback, string[] fileTypes, string defaultPath)
  {
    if (IsBusy)
      return false;

    Instance.FileMode = FileModeEnum.OpenFiles;
    Instance.Filters = fileTypes;
    Instance.CurrentPath = defaultPath;

    void ForwardMultiFiles(string[] filePaths)
    {
      callback?.Invoke(filePaths);
      Unsubscribe();
    }

    void Unsubscribe()
    {
      Instance.FilesSelected -= ForwardMultiFiles;
      Instance.Canceled -= Unsubscribe;
      IsBusy = false;
    }

    IsBusy = true;
    Instance.FilesSelected += ForwardMultiFiles;
    Instance.Canceled += Unsubscribe;
    Instance.PopupCentered();
    return true;
  }
}
