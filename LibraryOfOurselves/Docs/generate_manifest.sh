#!/usr/bin/env bash

ROOT="Content"
OUTPUT="manifest.json"

echo "Generating manifest..."

# start JSON
echo "{" > "$OUTPUT"
echo '  "version": 1,' >> "$OUTPUT"
echo '  "files": [' >> "$OUTPUT"

first=1

find "$ROOT" -type f | while read -r file; do
    rel="${file#./}"

    size=$(stat -c%s "$file" 2>/dev/null || stat -f%z "$file")

    # sha256 (mac + linux compatible)
    if command -v sha256sum >/dev/null 2>&1; then
        hash=$(sha256sum "$file" | awk '{print $1}')
    else
        hash=$(shasum -a 256 "$file" | awk '{print $1}')
    fi

    if [ $first -eq 0 ]; then
        echo "    ," >> "$OUTPUT"
    fi
    first=0

    cat >> "$OUTPUT" <<EOF
    {
      "path": "$rel",
      "size": $size,
      "sha256": "$hash"
    }
EOF

done >> "$OUTPUT"

echo "" >> "$OUTPUT"
echo "  ]" >> "$OUTPUT"
echo "}" >> "$OUTPUT"

echo "Done -> $OUTPUT"