# PROGRESS.md — Project Rolling 進度紀錄

> context 被清掉後從這裡接續工作。每次完成任務後更新對應段落。最後更新：2026-10-31（Engineer）

## 目前階段
早期 prototype（單一場景 `Prototype.unity`）。核心移動、撿放物品、路人生成、車輛往返、倒數計時已有雛形；尚無勝負/計分流程。
本輪：建立團隊文件（CLAUDE.md / PROGRESS.md）並評估 Unity MCP 與引擎升級。

## 團隊角色
| 角色 | 負責 |
|---|---|
| PM（Claude） | 派工、協調、git commit/push |
| Gameplay Designer（Codex） | 遊戲系統企劃 |
| Designer（Impeccable plugin） | 視覺 |
| Engineer（Claude） | 程式實作、技術評估 |

## 已完成
| Commit | 內容 |
|---|---|
| `ecf876b` | Initial commit |
| `3b7c7b7` | 建立專案：基本移動、撿/放物品、倒數計時 |
| `f4a18e9` | 修正 NavMeshAgent 追蹤錯誤 |
| `54d8de9` | Somebody 在玩家靠近時避開（觸發目前已被註解，見未解問題） |
| `ac12191` | 隱形牆、擴大地板 |
| `de9aaa8` | 車輛以 NavMeshAgent 移動 |
| `66c4a2d` | 不用 collider 改用 clamp 限制玩家移動範圍；修正車輛重生 |
| `2abb1ce` | 修正 SpawnManager 生成 Somebody 的問題 |
| （未 commit） | 新增 `CLAUDE.md`、`PROGRESS.md`；完成 MCP/升級評估 |

## 決策

### [待使用者決定] Unity MCP 與引擎升級
**MCP 選項比較**

| 選項 | 最低 Unity 版本 | 能用於 2019.4？ | 設定成本 |
|---|---|---|---|
| CoplayDev/unity-mcp（MCP for Unity） | 2021.3 LTS（package.json 宣告） | ❌ | 低：UPM 用 git URL 安裝 + Python 3.10 / uv，有自動設定 Claude Code 的視窗。免費、開源、社群最活躍 |
| Unity 官方（`com.unity.ai.assistant` 內建 Unity MCP Server） | Unity 6（6000.0）+ | ❌ | 低：裝套件後 bridge 自動啟動，client 指向 `~/.unity/relay/relay_win.exe --mcp`，首次連線需在 Project Settings 核准。官方維護；需確認是否需要 Unity 帳號/AI 方案 |
| IvanMurzak/Unity-MCP | 2022.3（舊版文件寫 2021.3） | ❌ | 中 |
| CoderGamester/mcp-unity | 2021.3 或 Unity 6（來源不一致） | ❌ | 中：需 Node.js 18+ |
| ibelongedtoyou/unity-mcp-2019（CoplayDev 的社群 fork） | 2019.4 | ⚠️ 理論上可以 | 中；**未經 CoplayDev 背書、無回報、完整度未知**，不建議 |

結論：沒有任何主流 MCP 支援 2019.4，要用 MCP 基本上必須升級。

**升級到新版 LTS 的風險評估**（專案很小：6 支腳本、1 場景、2 prefab，沒有第三方套件）

| 項目 | 2021.3 | 2022.3 | Unity 6 |
|---|---|---|---|
| Built-in RP | 正常 | 正常 | 仍可用（Unity 已表示未來會淘汰，但目前不影響） |
| NavMesh（`NavMeshAgent` + 已烘焙的 `NavMesh.asset`） | 正常 | runtime 正常；舊的 Navigation 烘焙視窗變成「Obsolete」，要重新烘焙建議裝 `com.unity.ai.navigation` | 同 2022.3，重新烘焙應改用 AI Navigation 套件（NavMeshSurface） |
| 舊版 Input Manager | 正常 | 正常 | 正常（升級的專案會保留 Old input；只有新專案預設 Input System） |
| `UnityEngine.UI.Text` | 正常 | 正常 | 仍可用，但在 uGUI 2.0 中標為 Legacy；TMP 併入 ugui 套件 |
| 套件 | collab-proxy/ide 套件自動更新 | 同左；`ide.vscode` 已淘汰可移除 | TMP/ugui 合併、collab-proxy → Unity Version Control，自動處理 |
| 腳本 API | 沒有會壞的 API | 同左 | 同左（目前用的 API 都還在） |

預期會壞/要處理的：只有重新烘焙 NavMesh 的流程（2022.3+）、套件版本更新、Android 建置設定（最低 API level / Gradle 變更，目前還沒在建置所以影響小）。升級會重新序列化所有資產，**不可逆**，必須在 git 分支上做。整體風險：**低**。

**建議（Engineer）**：在新分支升級到**最新的 Unity 6 LTS**，同時裝 **CoplayDev/unity-mcp**（免費、不綁 Unity AI 方案、文件與社群最完整）；Unity 官方 MCP 當備案。理由：專案早期、內容少，越晚升級成本越高；Unity 6 是目前主要支援的 LTS，兩種主流 MCP 都能用。若使用者不想升級：不要用 MCP，維持「Claude 改腳本、使用者在 Editor 裡測試」的流程（不建議用 2019 社群 fork）。
→ **等使用者決定**：(a) 升級到 Unity 6 LTS + CoplayDev MCP（建議）；(b) 升級到 2022.3 LTS；(c) 維持 2019.4，不用 MCP。

## 未解問題
1. **Player 落地問題**：`Player.Moving()` 中 `_yVelocity` 不在地面時會一直累加（`-= 1f`），但從來沒有套用到 `move`，所以重力不會生效；而且落地後也不會重設，每幀累加也沒乘 `Time.deltaTime`。
2. **SpawnManager 寫死範圍**：`Random.Range(0, 3)` 應改成 `_startPosition.Length` / `_endPosition.Length`，否則陣列不是 3 個時會越界或用不到某些點。
3. **Timer 結束後沒有行為**：時間到只設 `GameManager.IsTimeGoing = false` 並 log，沒有任何系統讀取它（生成/車輛/玩家都不會停），也沒有結算。
4. **Somebody 躲避未啟用**：`OnTriggerEnter` 被註解掉，`_isSomebodyNoticed` 永遠不會變成 true；`_detectRange` 只用在 Gizmo；`_controller` 欄位沒用到。
5. （次要）Player 旋轉用 `_speed` 當每幀旋轉角度，速度跟旋轉耦合且跟 frame rate 有關。
6. （次要）CarMoves 每幀呼叫 `SetDestination`。

## 下一步
1. 使用者決定 MCP/升級方案（見決策）。
2. 修正未解問題 1–2（Engineer，小改動）。
3. Gameplay Designer 定義 Timer 結束後的遊戲流程（勝負/計分），再由 Engineer 實作問題 3。
4. 確認 Somebody 躲避玩家的設計後重新啟用（問題 4）。
