
# Build Android APK (Signed)

dotnet build "LehrplanGenerator.Android/LehrplanGenerator.Android.csproj" -f net10.0-android -c Release -p:AndroidPackageFormat=apk -p:AndroidSdkDirectory="C:/Users/olive/AppData/Local/Android/Sdk" -p:AndroidKeyStore=true -p:AndroidSigningKeyAlias=androiddebugkey -p:AndroidSigningKeyPass=android -p:AndroidSigningStorePass=android -p:AndroidSigningKeyStore="C:/Users/olive/.android/debug.keystore"


# Install APK on Virtual Device

adb install -r LehrplanGenerator.Android/bin/Release/net10.0-android/com.CompanyName.LehrplanGenerator-Signed.apk


# Uninstall APK from Virtual Device

adb uninstall com.CompanyName.LehrplanGenerator
