#!/usr/bin/env bash

ROOT="Content"
OUTPUT="manifest.json"

echo "Generating manifest..."

echo "{" > "$OUTPUT"
echo '  "version": 1,' >> "$OUTPUT"
echo '  "guideFiles": [' >> "$OUTPUT"

firstGuide=1
firstUser=1

# Temporary files for each category
GUIDE_TMP=$(mktemp)
USER_TMP=$(mktemp)
find "$ROOT" -type f | while read -r file; do
    rel="${file#./}"
    filename=$(basename "$file")

    size=$(stat -c%s "$file" 2>/dev/null || stat -f%z "$file")

    if command -v sha256sum >/dev/null 2>&1; then
        hash=$(sha256sum "$file" | awk '{print $1}')
    else
        hash=$(shasum -a 256 "$file" | awk '{print $1}')
    fi

    entry=$(cat <<EOF
    {
      "filename": "$filename",
      "size": $size,
      "sha256": "$hash"
    }
EOF
)

    if [[ "$rel" == Content/Guide/* ]]; then
        if [ -s "$GUIDE_TMP" ]; then
            echo "," >> "$GUIDE_TMP"
        fi
        echo "$entry" >> "$GUIDE_TMP"

    elif [[ "$rel" == Content/User/* ]]; then
        if [ -s "$USER_TMP" ]; then
            echo "," >> "$USER_TMP"
        fi
        echo "$entry" >> "$USER_TMP"
    fi

done

cat "$GUIDE_TMP" >> "$OUTPUT"

echo "" >> "$OUTPUT"
echo "  ]," >> "$OUTPUT"
echo '  "userFiles": [' >> "$OUTPUT"

cat "$USER_TMP" >> "$OUTPUT"

echo "" >> "$OUTPUT"
echo "  ]" >> "$OUTPUT"
echo "}" >> "$OUTPUT"

rm "$GUIDE_TMP" "$USER_TMP"

echo "Done -> $OUTPUT"