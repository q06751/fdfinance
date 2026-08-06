#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
cat c*.b64 | base64 -d > fdfinance-complete.tar.gz
echo "wrote fdfinance-complete.tar.gz ($(wc -c < fdfinance-complete.tar.gz) bytes)"
echo "extract with: tar -xzf fdfinance-complete.tar.gz"
