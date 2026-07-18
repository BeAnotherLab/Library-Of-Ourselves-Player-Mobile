#!/usr/bin/env bash

ROOT="Content"
OUTPUT="manifest.json"

# Size of chunk in bytes (10MB)
CHUNK_SIZE=10485760

echo "Generating manifest..."

echo "{" > "$OUTPUT"
echo '  "version": 1,' >> "$OUTPUT"
echo '  "guideFiles": [' >> "$OUTPUT"

GUIDE_TMP=$(mktemp)
USER_TMP=$(mktemp)

find "$ROOT" -type f | while read -r file; do
    rel="${file#./}"
    filename=$(basename "$file")

    # Get file size safely on Linux or macOS
    size=$(stat -c%s "$file" 2>/dev/null || stat -f%z "$file")

    echo "Checksumming: $file"

    if [ "$size" -le $((CHUNK_SIZE * 2)) ]; then
        # File is small, hash the whole thing directly
        if command -v sha256sum >/dev/null 2>&1; then
            hash=$(sha256sum "$file" | awk '{print $1}')
        else
            hash=$(shasum -a 256 "$file" | awk '{print $1}')
        fi
    else
        # File is large: extract the first 10MB and last 10MB into a fast stream, then hash it
        hash=$( (head -c "$CHUNK_SIZE" "$file"; tail -c "$CHUNK_SIZE" "$file") | \
                (sha256sum 2>/dev/null || shasum -a 256) | awk '{print $1}' )
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
