#!/bin/bash
# 주의사항
# Window에서 파일 수정후 맥에서 실행시 줄바꿈 코드(EOL)를 Unix 형식으로 변경바랍니다. (Window/MAC은 안됨)

APP_STORE_APIKEY=$1
APP_STORE_ISSURE_ID=$2
IPA_PATH=$3 #= ${WORKSPACE}/Build/Project.ipa

echo "Validate App IPA_PATH : $IPA_PATH"
xcrun altool --validate-app --type ios --file "$IPA_PATH" --apiKey "$APP_STORE_APIKEY" --apiIssuer "$APP_STORE_ISSURE_ID"

echo "Upload App IPA_PATH : $IPA_PATH"
xcrun altool --upload-app --type ios --file "$IPA_PATH" --apiKey "$APP_STORE_APIKEY" --apiIssuer "$APP_STORE_ISSURE_ID"