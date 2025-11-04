#!/usr/bin/env bash

# Base directory in home
BASE_DIR="$HOME/DMS"

# Folder structure array
DIRS=(
  "$BASE_DIR/Delius/Input"
  "$BASE_DIR/Delius/Output"
  "$BASE_DIR/Offloc/Input"
  "$BASE_DIR/Offloc/Output"
)

echo "Creating DMS folder structure..."

for dir in "${DIRS[@]}"; do
    if [ ! -d "$dir" ]; then
        mkdir -p "$dir"
        echo "📁 Created: $dir"
    else
        echo "Already exists: $dir"
    fi
done

echo "🎉 Setup complete!"