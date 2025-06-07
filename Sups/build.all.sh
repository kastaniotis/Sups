#!/bin/bash
set -e  # Exit on error

APP_NAME="sups"
OUTPUT_DIR="./public"

# Configuration per RID
declare -A CONFIGS
declare -A OBJCOPIES

CONFIGS["linux-x64"]="-p:PublishAot=true"
CONFIGS["linux-arm64"]="-p:PublishAot=true"
CONFIGS["linux-arm"]="-p:PublishSingleFile=true"

# Matching objcopy tools for each target
OBJCOPIES["linux-x64"]="objcopy"
OBJCOPIES["linux-arm64"]="aarch64-linux-gnu-objcopy"
OBJCOPIES["linux-arm"]="arm-linux-gnueabihf-objcopy"

for RID in "linux-x64" "linux-arm64" "linux-arm"; do
  echo -e "\n=== Building for $RID ==="
  
  ARCH=${RID#linux-}
  OUT_PATH="$OUTPUT_DIR/$ARCH"
  
  mkdir -p "$OUT_PATH"

  # Export the correct objcopy if needed
  export OBJCOPY=${OBJCOPIES[$RID]}

  # Warn if the objcopy tool is not found
  if ! command -v "$OBJCOPY" &>/dev/null; then
    echo "❌ Error: Required objcopy tool '$OBJCOPY' not found. Please install it (e.g., 'sudo apt install binutils-${OBJCOPY%-*}')"
    exit 1
  fi

  dotnet publish -r $RID -o "$OUT_PATH" \
    -c Release -p:DebugType=none \
    -p:PublishTrimmed=true \
    --self-contained \
    ${CONFIGS[$RID]}

  echo "✅ Build complete: $RID"

  echo "📦 Packaging ${APP_NAME} for $ARCH..."
  tar -czvf "$OUT_PATH/${APP_NAME}.tar.gz" -C "$OUT_PATH" "$APP_NAME"
done

echo -e "\n🎉 All builds completed successfully."
