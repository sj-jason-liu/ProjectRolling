# 轉移到 Mac 的方案

> 狀態：**已完成**（2026-11-05 確認）。本文件保留為搬遷紀錄；目前狀態以 `PROGRESS.md` 為準。

## 一、在 Windows 端先做

1. **處理未提交的 `.mcp.json`**：目前被清空成 `{"mcpServers": {}}`。要還原成 commit 版（UnityMCP → `http://localhost:8080/mcp`）執行 `git checkout .mcp.json`；要保留清空版就 commit。
2. **確認 push 完成**：`upgrade/unity6` 需與 `origin/upgrade/unity6` 同步。`main` 尚未合併升級版（等使用者的 Editor 測試通過）。
3. **備份記憶資料夾**：`C:\Users\washi\.claude\projects\C--Unity-Projects-Project-Rolling\memory\`（內含 PM 角色、herdr 分工、先 `/clear` 再派工、push 帳號、開工先確認待決事項等規則；不在 repo 中）。用 AirDrop／雲端／隨身碟帶走。

## 二、Mac 上：專案

```bash
git clone https://github.com/sj-jason-liu/ProjectRolling.git
cd ProjectRolling && git checkout upgrade/unity6
```

- Unity Hub 安裝 **6000.3.27f1**（版本須完全一致，Apple Silicon 版 Editor）。
- 第一次開啟會重新匯入 `Library/`（未追蹤），需等待。
- MCP：Window → MCP for Unity → Start Server；Mac 需 `brew install uv`。`.mcp.json` 不必改。
- CLAUDE.md 中 Windows 專屬內容要改：headless 編譯路徑改為 `/Applications/Unity/Hub/Editor/6000.3.27f1/Unity.app/Contents/MacOS/Unity`；`winget` 說明改 brew。（可交 Engineer 處理）
- GitHub push 必須用 **sj-jason-liu** 帳號（`mouse76229` 會 403）。可用 `gh auth login` 或 Keychain，登入時選對帳號。

## 三、Mac 上：Claude / Codex 環境

1. 安裝 Claude Code 並登入。
2. 安裝 Codex CLI 並登入（Gameplay Designer 刻意使用 Codex）。
3. 還原記憶：先在新專案資料夾跑一次 `claude`，讓它建立 `~/.claude/projects/<編碼後路徑>/`，再把備份的 `memory/` 放進去。記憶中 Git Bash 路徑轉換（`/clear` 被轉成路徑）的 gotcha 在 Mac 不適用。
4. 重裝 Impeccable（Designer 用）：`/plugin` → 加入 marketplace `pbakaus/impeccable` → 安裝 `impeccable@impeccable`。
5. herdr hook：Windows 的 `settings.json` 有 SessionStart hook 呼叫 `herdr-agent-state.ps1`（PowerShell），**不要複製**。改用 herdr 在 Mac 提供的 Claude 整合安裝方式（查 `herdr --help`／官方文件，指令名稱待確認）。
6. `settings.json` 其他設定（如 `"model": "sonnet"`）手動重設，不整檔複製。

## 四、Mac 上：herdr 分工

Windows 的 `%APPDATA%\herdr\session.json` 綁 pane ID 與 socket，不可搬移，需在 Mac 重建 tab（工作目錄皆為專案根目錄，先開 PM）：

| Tab | Agent | 職責 |
|---|---|---|
| PM | claude | 接使用者指示、派工、整合回報、負責 commit/push |
| Gameplay Designer | **codex** | 遊戲架構與系統規劃，與使用者確認方向 |
| Designer | claude + Impeccable | 視覺設計，提出不同設計方向 |
| Engineer | claude | 主工程師，讓專案能 build 與執行 |
| IT | — | 暫不使用，可不建 |

建好後在 PM 跑 `herdr tab list`／`herdr pane list` 確認新 pane ID。派工流程不變：`/clear` 目標 → `herdr agent prompt <pane> "..." --wait`（prompt 須自足）；各角色回報給 PM，PM 統整後回報使用者。

## 五、第一次開 PM 時

PM 讀 `PROGRESS.md` 並重新確認待決事項：(A) Unity 6 升級與 MCP 的 Editor 測試（建議直接在 Mac 上測，通過後合併 main）；(B) `Docs/Design/GameLoop.md` 的 6 個設計問題；(C) `.mcp.json` 處理（Windows 端第 1 步完成後即解決）。
