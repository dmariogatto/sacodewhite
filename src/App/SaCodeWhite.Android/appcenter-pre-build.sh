﻿#!/bin/bash

manifestPath="${BUILD_REPOSITORY_LOCALPATH}/src/App/SaCodeWhite.Android/Properties/AndroidManifest.xml"
buildNumber=$APPCENTER_BUILD_ID
buildOffset=$BUILD_ID_OFFSET
newVersionName="$VERSION_NAME"

# exit if a command fails
set -e

if [ ! -f $manifestPath ] ; then
  echo " [!] File doesn't exist at specified AndroidManifest.xml path: ${manifestPath}"
  exit 1
fi

verCodeCmd="grep versionCode ${manifestPath} | sed 's/.*versionCode=\"//;s/\".*//'"
verNameCmd="grep versionName ${manifestPath} | sed 's/.*versionName\s*=\s*\"\([^\"]*\)\".*/\1/g'"

versionCode=$(eval $verCodeCmd)
versionName=$(eval $verNameCmd)

newVersionCode=$((buildNumber + buildOffset))

if [ -z "$newVersionName" ] ; then
    newVersionName="$(date -u +'%Y.%-m.%-d')"
fi

echo " (i) Provided AndroidManifest.xml path: ${manifestPath}"

# verbose / debug print commands
set -v

# ---- Current Version Code:
echo $versionCode
# ---- Set Version Code:
sed -i.bak "s/android:versionCode="\"${versionCode}\""/android:versionCode="\"${newVersionCode}\""/" ${manifestPath}
# ---- New Version Code:
eval $verCodeCmd

# ---- Current Version Name:
echo $versionName
# ---- Set Version Name:
sed -i.bak "s/android:versionName="\"${versionName}\""/android:versionName="\"${newVersionName}\""/" ${manifestPath}
# ---- New Version Name:
eval $verNameCmd

# ==> Manifest Version Code and Name changed

scriptPath="${BUILD_REPOSITORY_LOCALPATH}/src/App/appcenter-replace-settings.sh"
settingsPath="${BUILD_REPOSITORY_LOCALPATH}/src/App/SaCodeWhite/settings.json"

chmod u+x $scriptPath

$scriptPath $settingsPath

sed -i.bak "s#{AppCenterAndroidSecret}#$APPCENTER_ANDROID_SECRET#g" $settingsPath

cat $manifestPath
cat $settingsPath

constantsPath="${BUILD_REPOSITORY_LOCALPATH}/src/App/SaCodeWhite/Constants/Constants.cs"

if [ ! -f $constantsPath ] ; then
  echo " [!] File doesn't exist at specified path: $constantsPath"
  exit 1
fi
