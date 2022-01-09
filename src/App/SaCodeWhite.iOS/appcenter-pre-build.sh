#!/bin/bash

scriptPath="${BUILD_REPOSITORY_LOCALPATH}/src/App/SaCodeWhite.iOS/appcenter-update-bundle-version.sh"
appPlistPath="${BUILD_REPOSITORY_LOCALPATH}/src/App/SaCodeWhite.iOS/Info.plist"

chmod u+x $scriptPath

$scriptPath "${appPlistPath}" $APPCENTER_BUILD_ID $BUILD_ID_OFFSET "$VERSION_NAME"

sed -i.bak "s#{AdMobApplicationId}#$ADMOB_APPLICATION_ID#g" $appPlistPath

scriptPath="${BUILD_REPOSITORY_LOCALPATH}/src/App/appcenter-replace-settings.sh"
settingsPath="${BUILD_REPOSITORY_LOCALPATH}/src/App/SaCodeWhite/settings.json"

chmod u+x $scriptPath

$scriptPath $settingsPath

sed -i.bak "s#{AppCenterIosSecret}#$APPCENTER_IOS_SECRET#g" $settingsPath

cat $appPlistPath
cat $settingsPath
