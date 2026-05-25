# 对话系统使用说明

本文件夹包含一套基于 **ScriptableObject（`DialogueNode`）** 的轻量对话：场景中由 **`DialogueManager`** 驱动流程，**`DialogueUI`** 负责显示与按钮，**`DialogueLauncher`** 作为对外入口（按钮或脚本调用）。

---

## 1. 场景里需要有什么

| 组件 | 作用 |
|------|------|
| **DialogueManager** | 单例，维护当前节点、发事件、处理「继续」与选项跳转。场景中应只有 **一个**。 |
| **DialogueUI** | 订阅 Manager 事件，显示姓名、正文、立绘、继续按钮与选项按钮；对话开始显示根物体，结束隐藏。 |
| **DialogueLauncher**（可选） | 封装 `Begin` / `Stop`、开始与结束时的 UnityEvent / C# 事件。 |

**启动顺序**：`DialogueUI` 在 `Start` 里会访问 `DialogueManager.Instance`，请保证 **Manager 的 `Awake` 先于 UI 的 `Start` 执行**（通常把 Manager 放在层级里更靠上，或挂在同一物体上且脚本顺序 Manager 在 UI 之前）。

---

## 2. 对话数据：`DialogueNode` 资源

在 Project 窗口：**Create → Dialogue → DialogueNode**，得到 `.asset` 文件。

每个节点可配置：

- **Speaker Name**：显示在姓名栏。
- **Character Id**：立绘查表用；**留空**时 UI 会用 `speakerName` 作为查表键。
- **Dialogue Text**：当前句正文。
- **Options**：选项列表（见下）。
- **Next Node**：**无选项**时，玩家点「继续」后进入的下一节点；最后一节可留空表示对话结束。

### 2.1 线性对话（只有继续、没有分支）

- `options` 留空或数量为 0。
- 用 **Next Node** 串成链：A → B → C；最后一句的 **Next Node** 为空则结束。

### 2.2 分支对话（有选项）

- 在 **Options** 里添加多条，每条填写：
  - **Option Text**：选项文案（会写入按钮上的 **TextMeshProUGUI**）。
  - **Target Node**：选中后跳转到的节点。
  - **Option Button Prefab**（可选）：该选项专用按钮预制体；**留空**则用 `DialogueUI` 上的默认选项按钮预制体。
- 若某选项的 **Target Node** 为空，选中后会 **直接结束整段对话**。

---

## 3. `DialogueUI` Inspector 绑定要点

- **Speaker Name Text / Dialog Text**：`TextMeshProUGUI`。
- **Portrait Image / Portrait Database**：可选；数据库为 **Create → Dialogue → Character Portrait Database**，其中每条 **Character Id** 需与节点上的 `characterId`（或回退用的 `speakerName`）一致（不区分大小写）。
- **Options Parent**：选项按钮实例化的父级 `Transform`。
- **Option Button Prefab**：默认选项按钮预制体，须满足：
  - 子层级中有 **`TextMeshProUGUI`**（用于显示 `optionText`）；
  - 有 **`Button`**（可挂在根或子物体上）。
- **Continue Button**：无选项时显示的继续按钮，点击会调用 `DialogueManager.Advance()`。

**选项不显示文字**：常见原因是预制体里没有激活的 **TMP** 组件、用了旧版 **UI Text**、或 `DialogueOption.optionText` 在资源里为空。详见控制台 Warning/Error。

---

## 4. `DialogueLauncher`：如何开始 / 结束对话

### 4.1 Inspector

- **Default Start Node**：无参 **Begin()** 时使用的起点 `DialogueNode`。
- **On Dialogue Began / On Dialogue Ended**：UnityEvent，可绑音效、任务标记等。

### 4.2 代码

```csharp
// 使用 Inspector 里配置的默认起点
launcher.Begin();

// 指定起点（例如随机多套固定稿中的一套）
launcher.Begin(someDialogueNodeAsset);

// 打断当前对话（若由本 Launcher 开启，仍会收到结束回调）
launcher.Stop();

// 运行时改默认起点再无参开始
launcher.DefaultStartNode = anotherNode;
launcher.Begin();
```

**注意**：若已有对话在进行、或上一轮 Launcher 会话尚未结束，再次 `Begin` 会被忽略并打 Warning。

---

## 5. 不经过 Launcher、直接开对话

若场景里已有 `DialogueManager`：

```csharp
DialogueManager.Instance.StartDialogue(startNode);
```

结束可调用 `DialogueManager.Instance.StopDialogue()`。自行订阅 `OnDialogueEnded` 等事件时，注意在对象销毁时取消订阅，避免泄漏。

---

## 6. 多套固定内容「随机一种」的做法（概念）

每种内容一套 **独立的 `DialogueNode` 根资源**（各自链或分支写死在 asset 里）。在调用 `Begin` 前由你的逻辑 **随机选一个根节点**（或按任务条件选），再 `Begin(选中的根)` 或赋值 `DefaultStartNode` 后 `Begin()`。无需改 Manager/UI 核心逻辑，可加一个很薄的「随机路由器」脚本专门负责选节点。

---

## 7. 预制体参考

- **OptionButton.prefab**：默认选项按钮示例，含 TMP 子物体与 Button，可在 `DialogueUI` 的 **Option Button Prefab** 中引用。

---

## 8. 文件一览

| 文件 | 说明 |
|------|------|
| `DialogueNode.cs` | 节点与选项数据结构 + 效果绑定 |
| `DialogueManager.cs` | 流程、会话上下文与选项效果 |
| `DialogueSessionContext.cs` | 入口来源（联系人/来电等） |
| `DialogueEffectRouter.cs` | 按通道执行效果 |
| `DialogueUI.cs` | UI 与选项实例化 |
| `DialogueLauncher.cs` | 对外入口、`Begin` / `Stop`、事件 |
| `CharacterPortraitDatabase.cs` | 角色 ID → 立绘 |
| `OptionButton.prefab` | 默认选项按钮预制体 |

示例场景若工程中有 `Assets/Scenes/Dialogue.unity`，可对照其中的 `DialogueManager`、`DialogueUI`、`Launcher` 布置方式。

---

## 9. 常见问题：Launch / 继续 / 选项点不了

1. **DialogueUI 根物体 RectTransform 的 Scale 为 (0,0,0)**  
   其下所有子物体（含 Launcher、选项、继续）的 UI 交互会异常。请将 **Scale 改为 (1,1,1)**。

2. **`DialogueUI` 的 Option Button Prefab 为 Missing 或 GUID 错误**  
   应拖入 `Assets/Code/DialogueSystem/OptionButton.prefab`，否则有选项时无法正确生成按钮。

3. **场景缺少 `EventSystem`**，或 **Canvas 未启用 `Graphic Raycaster`**（本场景的 DialogueUI Canvas 已带 Raycaster，勿删）。

4. **全屏 Image 打开 Raycast Target** 且盖在上层，会挡住点击；可调层级或关闭该 Image 的 Raycast Target。

---

## 10. BB 机系统（BBPhone）— 需你本人在 Unity 里操作的部分

**原则：代码不会在运行时生成任何 UI 或按钮。** 所有 Image、Button、TMP 请你在场景或 Prefab 里摆好，再拖引用；贴图、Sprite、Animator、音效均由你在 Inspector 替换。

### 10.1 Project 里 Create 资源（拖数据，不写代码）

| 菜单 | 用途 |
|------|------|
| **BBPhone → Contact Profile** | 一个联系人：名字、列表头像 Sprite、默认 `DialogueNode` |
| **BBPhone → Contact Database** | 把多个 Contact Profile **拖进列表** |
| **BBPhone → General Incoming Pool** | 与监控无关的随机来电：拖多个 `DialogueNode` 根 |
| **BBPhone → Monitor Incoming Config** | 按客人总数/特殊客人数区间匹配：每条拖一个 `DialogueNode` |
| **BBPhone → UI Theme** | 选项槽 **未选中/选中** 背景 Sprite（可再加联系人高亮 Sprite） |
| **Dialogue → DialogueNode** | 对话内容与选项（每节点 **最多 2 个 Option**） |

### 10.2 场景 Hierarchy（你要自己搭 UI）

建议结构（名称随意，引用拖对即可）：

```
Canvas（常驻 Active，不要拖给 bbMachineRoot）
├── Ring / RingButtonView   ← 仅来电响铃时显示；打开 BB 机后隐藏，直至下次 TriggerIncomingCall
└── BBMachineRoot           ← 拖给 BBPhoneController.bbMachineRoot
    ├── ContactListPanel    ← ContactListView（联系人界面）
│   │   ├── ContactName     TMP
│   │   ├── ContactAddress  TMP（住所）
│   │   ├── ContactPhone    TMP（电话）
│   │   ├── ContactIcon     Image（头像）
│   │   ├── BtnUp / BtnDown / BtnConfirm / BtnExit   Button + 自换 Image 贴图
│   └── DialoguePanel       ← BBDialogueView（对话界面）
│       ├── SpeakerName     TMP
│       ├── DialogueLine    TMP（一行正文）
│       ├── OptionsRow
│       │   ├── Option0Bg   Image + SpriteStateImage + Option0Text TMP
│       │   └── Option1Bg   Image + SpriteStateImage + Option1Text TMP
│       └── BtnUp / BtnDown / BtnConfirm / BtnExit（须在 bbMachineRoot 子树下）
```

**你要换贴图的地方：**

- 所有 **Button** 下的 **Image**：在 Inspector 换 Sprite（Up/Down/Confirm/Exit/响铃）。
- **RingButtonView**：`Idle Sprite`、`Ringing Sprite`；可选 **Animator**、**AudioSource**（铃声循环）。
- **BBPhoneUITheme** 资源：`optionSlotNormal` / `optionSlotSelected`（两个选项槽背景）。
- **ContactProfile** 每条：`listIcon`（联系人头像 Sprite）。
- **BBDialogueView** 的 `portraitImage`：立绘来自 `CharacterPortraitDatabase`（可选）。

### 10.3 场景组件挂载与拖引用

| 物体 | 组件 | 你要拖的内容 |
|------|------|----------------|
| 常驻 | **DialogueManager** | 仅一个 |
| 常驻 | **MonitorGuestProviderStub** | 测试用假 `Total/Special` 人数；监控接好后换 **MonitorGuestProviderLive** |
| 常驻 | **IncomingCallResolver** | Stub/Live、`GeneralIncomingPool`、`MonitorIncomingConfig` |
| BBPhoneRoot | **BBPhoneController** | `bbMachineRoot`（仅 BBMachineRoot）、`ringButtonRoot`（Ring）、三个 View、`ContactDatabase`、`IncomingCallResolver`；**Legacy Dialogue UI** 拖旧 DialogueUI 并禁用 |
| ContactListPanel | **ContactListView** | Database、Theme、4 个 Button、姓名/住所/电话 TMP、头像 Image |
| DialoguePanel | **BBDialogueView** | Theme、2 选项槽、4 个 Button、TMP |
| RingButton | **RingButtonView** | Button、Image、idle/ringing Sprite、Animator、AudioSource |

**BBPhoneController** 上把 **Legacy Dialogue UI** 指向旧 `DialogueUI` 物体，避免和 BB 对话界面重复显示。

### 10.4 操作逻辑（已实现，无需再写）

- **Tab** 或 **响铃按钮（不响铃时）**：打开 BB 联系人列表。
- **响铃中** Tab / 点 Ring：接听 → 打开 BB 机并**直接进入对话**（不经过联系人页）。
- **Idle 时点 Ring**：若无待接来电则先解析来电稿，同样打开 BB 机并直接进对话。
- **BB 已打开时来电**：直接进对话（调 `TriggerIncomingCall()`）。
- **对话结束**：自动关 BB 机面板。
- 联系人 **Up/Down**：循环切换（在首尾会绕回）；**Confirm** 进入该联系人 `defaultStartNode`。
- 对话 **Up/Down**：在两个选项间切换高亮；**Confirm** 确认或继续。

### 10.5 测试来电

- 挂 **BBPhoneIncomingTestButton**，Button OnClick → `TriggerIncomingCall()`；或代码调 `BBPhoneController.TriggerIncomingCall()`。
- 在 **MonitorGuestProviderStub** 改 `Stub Total/Special Guests`，配合 **Monitor Incoming Config** 条目区间测试监控相关对话。

### 10.6 监控同事接入后

只需：实现 **IMonitorGuestProvider**（或改 **MonitorGuestProviderLive**），替换 Stub；**GeneralIncomingPool / MonitorIncomingConfig / ContactDatabase 不用改**，继续拖 `DialogueNode` 即可。

### 10.7 选项效果（PolicePatrol / 结局 / 场景）

- 在 **DialogueNode** 的每个 **Option → On Select** 里添加 `Dialogue Effect Binding`：选 **Channel** + 拖入效果资源（Project 右键 **Dialogue → Effects → …**）。
- **PolicePatrol**：仅 **联系人入口** 对话执行，调用 `IPolicePatrolDialogueActions`（巡逻程序实现；测试可挂 `PolicePatrolDialogueActionsStub`）。
- **Narrative**：任意入口；`Game Over` 效果会在 Console 打 `gameover (endingId)`。
- **Scene**：`Load Scene` 可选立即加载或对话结束后加载（`SceneDialogueActionsDefault`）。
- 场景需有 **DialogueEffectServices**（与 DialogueManager 同物体或任意常驻物体），Inspector 拖入三个实现组件（或使用同物体上的 Default / Stub）。

### 10.8 BB 相关脚本一览

| 路径 | 说明 |
|------|------|
| `Integration/IMonitorGuestProvider.cs` | 监控读接口（来电选根） |
| `Integration/IPolicePatrolDialogueActions.cs` | 巡逻/监控写接口（联系人选项） |
| `Integration/PolicePatrolDialogueActionsStub.cs` | 巡逻占位 |
| `Integration/INarrativeDialogueActions.cs` | 结局 / GameOver |
| `Integration/ISceneDialogueActions.cs` | 切场景 |
| `Integration/MonitorGuestProviderStub.cs` | 假数据 |
| `Integration/MonitorGuestProviderLive.cs` | 正式接入空壳 |
| `Effects/*` | GameOver / LoadScene / PolicePatrol 等资源 |
| `DialogueEffectServices.cs` | 聚合接口 |
| `Data/*` | Contact / Pool / MonitorConfig / UITheme |
| `BBPhone/BBPhoneController.cs` | 总控 |
| `BBPhone/ContactListView.cs` | 联系人 UI |
| `BBPhone/BBDialogueView.cs` | 对话 UI（不 Instantiate） |
| `BBPhone/RingButtonView.cs` | 响铃按钮 |
| `BBPhone/IncomingCallResolver.cs` | 随机/监控解析 |
| `BBPhone/SpriteStateImage.cs` | 选项槽选中换 Sprite |
| `BBPhone/BBPhoneIncomingTestButton.cs` | 测试来电 |
