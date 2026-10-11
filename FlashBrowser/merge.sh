#!/usr/bin/env bash
set -Eeuo pipefail

ROOT="$(pwd -P)"
OUTPUT_NAME="${1:-merge_result.txt}"
OUTPUT_PATH="$ROOT/$OUTPUT_NAME"
TMP_FILE="$(mktemp)"
trap 'rm -f "$TMP_FILE"' EXIT

find "$ROOT" \
  \( -type d \( -name .git -o -name bin -o -name obj -o -name .vs -o -name .idea -o -name node_modules \) -prune \) -o \
  \( -type f -name '*.cs' ! -path "$OUTPUT_PATH" -print0 \) |
  sort -z |
  while IFS= read -r -d '' file; do
    rel_path="${file#"$ROOT"/}"
    {
      printf '\n// ============================================================================\n'
      printf '// Source file: %s\n' "$rel_path"
      printf '// ============================================================================\n\n'
      cat -- "$file"
      printf '\n\n'
    } >> "$TMP_FILE"
  done

if [[ ! -s "$TMP_FILE" ]]; then
  echo "Не найдено ни одного .cs-файла в: $ROOT" >&2
  exit 1
fi

mv -- "$TMP_FILE" "$OUTPUT_PATH"
trap - EXIT
printf 'Готово: %s\n' "$OUTPUT_PATH"