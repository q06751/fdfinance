# FdFinance 完整源码包

本目录提供 **一次性压缩包**，含全部 115 个源文件（backend + frontend + scripts + 启动配置）。

## 下载并解压

```bash
# 在仓库根目录
cd archive
bash extract.sh
# 生成 fdfinance-complete.tar.gz
cd ..
tar -xzf archive/fdfinance-complete.tar.gz
```

或直接：

```bash
cat archive/c*.b64 | base64 -d > fdfinance-complete.tar.gz
tar -xzf fdfinance-complete.tar.gz
```

## 校验

见 `MANIFEST.txt`（文件列表 + md5）。

## 启动

```bash
# API
cd backend && dotnet run --project src/FdFinance.Api

# Web
cd frontend && npm i && npm run dev
```

演示账号见根目录 README。
