# Full source archive

The complete FdFinance source tree is stored as base64 parts:

```bash
bash scripts/extract-full-source.sh
```

This reconstructs all backend / frontend / tests / dual-run scripts into the repo root.

Parts: `part_00.b64` … `part_03.b64` (concat → base64 -d → tar.gz).
