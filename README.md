# HeyAsync

Kleines WPF-Lernprojekt für `async/await`, `Task.Run`, `Dispatcher`, Race Conditions und `lock`.

## Ziel

Die Test-Buttons werden nicht mehr statisch in XAML definiert, sondern dynamisch aus Demo-Klassen erzeugt.

## Verwendete Dependencies

- `CommunityToolkit.Mvvm`
    - `ObservableObject`
    - `RelayCommand`
    - `AsyncRelayCommand`
- `Microsoft.Extensions.DependencyInjection`
    - Dependency Injection für ViewModels, Services und Demo-Klassen

Aktuelle NuGet-Versionen: `CommunityToolkit.Mvvm` 8.4.2 und `Microsoft.Extensions.DependencyInjection` 10.0.8. :contentReference[oaicite:0]{index=0}

## Architektur

```text
HeyAsync
├─ Demos
│  ├─ IAsyncDemo.cs
│  ├─ BasicAsyncDemo.cs
│  ├─ TaskRunDemo.cs
│  ├─ DispatcherDemo.cs
│  ├─ RaceConditionDemo.cs
│  └─ LockDemo.cs
├─ Services
│  ├─ IUiLogger.cs
│  └─ UiLogger.cs
├─ ViewModels
│  ├─ MainViewModel.cs
│  └─ AsyncDemoButtonViewModel.cs
├─ App.xaml
├─ App.xaml.cs
├─ MainWindow.xaml
└─ MainWindow.xaml.cs