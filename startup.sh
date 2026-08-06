#!/bin/sh
set -eu
cd /workspace

mkdir -p /workspace/data /workspace/logs /tmp/redis-data

redis_up() {
  python3 - <<'PY' 2>/dev/null
import socket
try:
    s = socket.create_connection(("127.0.0.1", 6379), 1)
    s.sendall(b"PING\r\n")
    data = s.recv(16)
    s.close()
    raise SystemExit(0 if b"PONG" in data else 1)
except Exception:
    raise SystemExit(1)
PY
}

if ! redis_up; then
  if [ -x /workspace/redis-server ]; then
    /workspace/redis-server --daemonize yes --port 6379 --dir /tmp/redis-data --dbfilename dump.rdb --save "" --appendonly no || true
  fi
fi

export PATH="${HOME}/.dotnet:/root/.dotnet:${PATH}"
export DOTNET_ROOT="${DOTNET_ROOT:-${HOME}/.dotnet}"
if [ ! -x "${DOTNET_ROOT}/dotnet" ] && [ -x /root/.dotnet/dotnet ]; then
  export DOTNET_ROOT=/root/.dotnet
  export PATH="/root/.dotnet:${PATH}"
fi

API_DLL="/workspace/backend/src/FdFinance.Api/bin/Debug/net10.0/FdFinance.Api.dll"
API_URLS="${API_URLS:-http://127.0.0.1:18765}"
API_HEALTH="${API_HEALTH:-http://127.0.0.1:18765/api/health}"

if ! curl -sf -o /dev/null --max-time 2 "$API_HEALTH"; then
  if [ ! -f "$API_DLL" ]; then
    cd /workspace/backend
    dotnet build src/FdFinance.Api/FdFinance.Api.csproj -v q
    cd /workspace
  fi
  cd /workspace/backend/src/FdFinance.Api
  nohup "${DOTNET_ROOT}/dotnet" bin/Debug/net10.0/FdFinance.Api.dll --urls "$API_URLS" \
    >>/workspace/logs/api.log 2>&1 &
  cd /workspace
  i=0
  while [ "$i" -lt 60 ]; do
    if curl -sf -o /dev/null --max-time 2 "$API_HEALTH"; then
      break
    fi
    i=$((i + 1))
    sleep 1
  done
fi

if curl -sf -o /dev/null --max-time 2 http://127.0.0.1:8080/; then
  exit 0
fi

cd /workspace/frontend
export API_URL="${API_URL:-http://127.0.0.1:18765}"
nohup npm run dev >>/workspace/logs/web.log 2>&1 &
exit 0
