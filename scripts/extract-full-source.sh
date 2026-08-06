#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"
shopt -s nullglob
parts=(archive/c*.b64)
if [ ${#parts[@]} -eq 0 ]; then
  echo "No archive/c*.b64 found" >&2
  exit 1
fi
cat "${parts[@]}" | tr -d '\n' | base64 -d > /tmp/fdfinance-full.tgz
tar -xzf /tmp/fdfinance-full.tgz -C "$ROOT"
echo "OK: full source extracted to $ROOT"
