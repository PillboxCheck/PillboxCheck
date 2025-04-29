# -*- mode: python ; coding: utf-8 -*-
from PyInstaller.utils.hooks import collect_all

datas_chroma, binaries_chroma, hiddenimports_chroma = collect_all('chromadb')
a = Analysis(
    ['RAGSocket.py'],
    pathex=['.'],
    binaries=[]+binaries_chroma,
    datas=[ ]+datas_chroma,
    hiddenimports=['socketprompts', 'utils','chatSocket','pydantic','pydantic-core','pydantic.deprecated.decorator',
    'tiktoken_ext.openai_public','tiktoken_ext','MySQLdb','pysqlite2','scipy.special._cdflib', 
    'chromadb.telemetry.product.posthog','chromadb','chromadb.telemetry','chromadb.api.rust','chromadb.api',
    'chromadb.utils.embedding_functions.onnx_mini_lm_l6_v2','onnxruntime','chromadb.api.segment','chromadb.db.impl',
    'tokenizers', 'chromadb.telemetry.posthog', 'chromadb.api.segment','chromadb.db.impl',
    'chromadb.db.impl.sqlite','chromadb.migrations', 'chromadb.migrations.embeddings_queue']+hiddenimports_chroma,
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    noarchive=False,
    optimize=0,
)
pyz = PYZ(a.pure)

exe = EXE(
    pyz,
    a.scripts,
    [],
    exclude_binaries=True,
    name='RAGSocket',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    console=True,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)
coll = COLLECT(
    exe,
    a.binaries,
    a.datas,
    strip=False,
    upx=True,
    upx_exclude=[],
    name='RAGSocket',
)
