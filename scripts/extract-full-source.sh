#!/usr/bin/env bash
set -euo pipefail
# Reconstruct full source from base64 parts committed under archive/
cd "$(dirname "$0")/.."
mkdir -p /tmp/fdfinance-extract
cat archive/part_*.b64 | tr -d '\n' | base64 -d > /tmp/fdfinance-full.tgz
tar -xzf /tmp/fdfinance-full.tgz -C .
echo "Extracted source into repo root."
