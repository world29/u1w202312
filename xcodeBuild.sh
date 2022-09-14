#!/bin/bash

SCHEME="Unity-iPhone"
PROJECT_PATH="${PROJECT_DIR}/${SCHEME}.xcodeproj"
ARCHIVE_FILE="${SCHEME}.xcarchive"
ARCHIVE_DIR="${PROJECT_DIR}/archive"
ARCHIVE_PATH="${ARCHIVE_DIR}/${ARCHIVE_FILE}"
IPA_DIR="${ARCHIVE_DIR}"
EXPORT_OPTIONS_PLIST="Build/ExportOptions.plist"
PROVISIONING_PROFILE="94e7f360-9668-4501-abfc-c6f8f43c47e5"

mkdir -p $ARCHIVE_PATH

# ARCHIVE
xcodebuild -project $PROJECT_PATH \
    -scheme $SCHEME \
    -destination 'generic/platform=iOS' \
    archive -archivePath $ARCHIVE_PATH \
    PROVISIONING_PROFILE=$PROVISIONING_PROFILE

# ipaファイルの作成
xcodebuild -exportArchive -archivePath $ARCHIVE_PATH \
    -exportPath $IPA_DIR \
    -exportOptionsPlist $EXPORT_OPTIONS_PLIST \
    PROVISIONING_PROFILE=$PROVISIONING_PROFILE
