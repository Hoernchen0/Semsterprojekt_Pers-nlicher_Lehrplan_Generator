# 📱 LernApp.Mobile - Plattformübergreifende Mobile-App

## Übersicht

Die Mobile-Version nutzt **.NET MAUI** (Multi-platform App UI) um eine native App für Android und iOS zu erstellen, die die gleichen Services wie Desktop und Web nutzt.

## Architektur

```
LernApp.Mobile/
├── Resources/
│   ├── Styles/
│   └── Images/
├── Views/
│   ├── LoginPage.xaml
│   ├── RegisterPage.xaml
│   ├── DashboardPage.xaml
│   ├── LernEinheitPage.xaml
│   ├── AIPage.xaml
│   └── SettingsPage.xaml
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── LernEinheitViewModel.cs
│   └── AIViewModel.cs
├── Models/
│   └── (Shared Models aus LernApp.Shared)
├── Services/
│   └── (API-Services für Server-Kommunikation)
├── App.xaml
├── App.xaml.cs
├── MauiProgram.cs
└── LernApp.Mobile.csproj
```

## Setup

### 1. MAUI-Projekt erstellen

```bash
dotnet new maui -n LernApp.Mobile
cd LernApp.Mobile
```

### 2. Abhängigkeiten

```bash
# NuGet-Pakete hinzufügen
dotnet add package Microsoft.Maui.Controls
dotnet add package Microsoft.Maui.Essentials
dotnet add package CommunityToolkit.Mvvm
dotnet add package HttpClientFactory
```

### 3. Services konfigurieren

In `MauiProgram.cs`:

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add Services
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<ILernplanApiService, LernplanApiService>();
        builder.Services.AddSingleton<IAIApiService, AIApiService>();

        // Add Pages & ViewModels
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddSingleton<DashboardViewModel>();

        return builder.Build();
    }
}
```

## Plattformspezifische Konfiguration

### Android

**File**: `Platforms/Android/AndroidManifest.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    
    <application>
        <!-- App Configuration -->
    </application>
</manifest>
```

### iOS

**File**: `Platforms/iOS/Info.plist`

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>NSLocalNetworkUsageDescription</key>
    <string>Diese App benötigt Internetzugriff</string>
    <key>NSBonjourServices</key>
    <array>
        <string>_http._tcp</string>
    </array>
</dict>
</plist>
```

## Bauen für verschiedene Plattformen

### Android

```bash
dotnet build -f net8.0-android
dotnet publish -f net8.0-android -c Release
```

APK wird generiert in: `bin/Release/net8.0-android/publish/`

### iOS

```bash
dotnet build -f net8.0-ios
dotnet publish -f net8.0-ios -c Release
```

### Windows (MAUI)

```bash
dotnet build -f net8.0-windows
dotnet publish -f net8.0-windows -c Release
```

## API-Integration

Die Mobile-App kommuniziert mit der Web-API statt direkt mit der Datenbank:

```csharp
public interface ILernplanApiService
{
    Task<IEnumerable<LernEinheit>> GetLernEinheitenAsync(int userId);
    Task<LernEinheit> CreateLernEinheitAsync(LernEinheit einheit);
    Task DeleteLernEinheitAsync(int id);
}

public class LernplanApiService : ILernplanApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.lernapp.de/api";

    public LernplanApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<LernEinheit>> GetLernEinheitenAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/lerneinheiten/{userId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<IEnumerable<LernEinheit>>();
    }

    public async Task<LernEinheit> CreateLernEinheitAsync(LernEinheit einheit)
    {
        var content = new StringContent(JsonSerializer.Serialize(einheit), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{BaseUrl}/lerneinheiten", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<LernEinheit>();
    }

    public async Task DeleteLernEinheitAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/lerneinheiten/{id}");
        response.EnsureSuccessStatusCode();
    }
}
```

## UI-Design

### Login Page

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="LernApp.Mobile.LoginPage"
             Title="LernApp">

    <VerticalStackLayout Padding="20" Spacing="20" VerticalOptions="Center">
        
        <Label Text="📚 LernApp" FontSize="32" HorizontalTextAlignment="Center" />
        <Label Text="Persönlicher Lehrplan Generator" FontSize="14" HorizontalTextAlignment="Center" />

        <Entry Placeholder="Email" Text="{Binding Email}" />
        <Entry Placeholder="Passwort" IsPassword="True" Text="{Binding Password}" />

        <Label Text="{Binding ErrorMessage}" TextColor="Red" />

        <Button Text="Anmelden" Command="{Binding LoginCommand}" />
        <Button Text="Registrieren" Command="{Binding RegisterCommand}" />

    </VerticalStackLayout>

</ContentPage>
```

## Testing

```bash
# Unit Tests
dotnet test LernApp.Mobile.Tests

# Android Emulator
dotnet build -f net8.0-android -c Debug
# Dann in Android Studio: AVD Manager -> Select Emulator -> Run
```

## Deployment

### Google Play Store

1. Erstelle Google Play Developer Account
2. Signiere APK mit Release-Zertifikat
3. Upload zu Google Play Console

### Apple App Store

1. Erstelle Apple Developer Account
2. Besorge Apple Developer Certificate
3. Erstelle App ID in App Store Connect
4. Upload via Xcode oder Transporter

## Nächste Schritte

- [ ] MAUI Projektstruktur einrichten
- [ ] Login/Register Pages implementieren
- [ ] API-Services erstellen
- [ ] Dashboard mit Liste der Lerneinheiten
- [ ] Offline-Sync-Funktionalität
- [ ] Push-Notifications
- [ ] Dark Mode Support

## Plattform-Kompatibilität

| Feature | Android | iOS | Windows | Mac |
|---------|---------|-----|---------|-----|
| Login | ✅ | ✅ | ✅ | ✅ |
| Lerneinheiten | ✅ | ✅ | ✅ | ✅ |
| KI-Chat | ✅ | ✅ | ✅ | ✅ |
| Offline-Modus | ✅ | ✅ | ⚠️ | ⚠️ |
| Benachrichtigungen | ✅ | ✅ | ❌ | ❌ |
