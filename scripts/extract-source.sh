#!/bin/sh
# Reconstruct full source from base64 parts
set -e
cd "$(dirname "$0")/.."
cat releases/fdfinance-source.tar.gz.b64.part* | base64 -d > fdfinance-source.tar.gz
tar xzf fdfinance-source.tar.gz
echo "Extracted. See README.md"
