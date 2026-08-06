#!/usr/bin/env bash
# TDD 守卫：先红后绿，防止“空断言 / 幻觉通过”
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
export PATH="${HOME}/.dotnet:/root/.dotnet:${PATH}"
export DOTNET_ROOT="${DOTNET_ROOT:-/root/.dotnet}"
cd "$ROOT"

PROJECT="tests/FdFinance.Tests/FdFinance.Tests.csproj"
HASHER="src/FdFinance.Application/Security/PasswordHasher.cs"
BACKUP="$(mktemp)"

echo "======== [1/3] GREEN：全量测试应通过 ========"
dotnet test "$PROJECT" --nologo -v q
echo "✓ GREEN phase passed"

echo "======== [2/3] RED：故意破坏 MD5 已知向量，测试必须失败 ========"
cp "$HASHER" "$BACKUP"
python3 - <<'PY'
from pathlib import Path
p = Path("src/FdFinance.Application/Security/PasswordHasher.cs")
t = p.read_text()
old = '''    public static string Md5Hex(string source)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(source));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }'''
new = '''    public static string Md5Hex(string source)
    {
        // INTENTIONAL BREAK for TDD red phase — restored by run_tests-tdd.sh
        return "00000000000000000000000000000000";
    }'''
if old not in t:
    raise SystemExit("cannot locate Md5Hex for red-phase mutation")
p.write_text(t.replace(old, new, 1))
print("mutated Md5Hex")
PY

set +e
dotnet test "$PROJECT" --nologo -v q --filter "FullyQualifiedName~PasswordHasherTests.Md5Hex_matches_known_vector"
RED_RC=$?
set -e
cp "$BACKUP" "$HASHER"
rm -f "$BACKUP"

if [ "$RED_RC" -eq 0 ]; then
  echo "✗ RED phase FAILED: mutation did not break tests (possible hallucination)"
  exit 1
fi
echo "✓ RED phase passed (tests correctly failed under mutation)"

echo "======== [3/3] GREEN：恢复后全量再跑 ========"
dotnet test "$PROJECT" --nologo -v q
echo "✓ Final GREEN passed"
echo "======== TDD 守卫完成：先绿 → 突变红 → 再绿 ========"
