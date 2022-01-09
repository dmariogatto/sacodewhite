#!/bin/bash

settingsPath=$1

# exit if a command fails
set -e

if [ ! -f $settingsPath ] ; then
  echo " [!] File doesn't exist at specified path: ${settingsPath}"
  exit 1
fi

sed -i.bak "s#{ApiUrlBase}#$API_BASE_URL#g" $settingsPath

sed -i.bak "s#{ApiKeyHospitals}#$API_KEY_HOSPITALS#g" $settingsPath
sed -i.bak "s#{ApiKeyTriageCategories}#$API_KEY_TRICATS#g" $settingsPath
sed -i.bak "s#{ApiKeyAmbulanceDashboards}#$API_KEY_AMBO_DASHBOARD#g" $settingsPath
sed -i.bak "s#{ApiKeyEmergencyDepartmentDashboards}#$API_KEY_ED_DASHBOARD#g" $settingsPath
sed -i.bak "s#{ApiKeyRegisterDevice}#$API_KEY_REG_DEVICE#g" $settingsPath
sed -i.bak "s#{ApiKeyDeleteDevice}#$API_KEY_DEL_DEVICE#g" $settingsPath

set +v