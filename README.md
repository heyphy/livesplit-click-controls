# livesplit-click-controls

LiveSplitのレイアウトに、実際のWinForms Buttonを3つ追加するComponentです。
表示名は **Click Controls**、対象は **.NET Framework 4.8.1** です。

- **Start / Split**: `TimerPhase.Running`なら`TimerModel.Split()`、それ以外は`Start()`。
  現行APIではPaused・Endedに対するStartは何もしません。
- **Undo**: `TimerModel.UndoSplit()`。
- **Reset**: `TimerModel.Reset()`。このComponentは確認ダイアログを追加しません。

すべて生成時に渡された`LiveSplitState`を操作します。設定画面やネットワーク処理はありません。

## ビルド

Windowsと.NET SDKが必要です（ローカル検証: SDK 7.0.410）。
.NET Framework 4.8.1 Developer Packがない場合、SDKが参照アセンブリをNuGetから復元するため、初回はネット接続が必要です。
LiveSplit本体のソースビルドには現行upstreamが指定する.NET 10 SDKが必要ですが、
このComponentは公式配布DLLを参照するため、本体のビルドは不要です。

リポジトリ直下でPowerShellから実行します。

```powershell
New-Item -ItemType Directory -Force .deps | Out-Null
Invoke-WebRequest https://raw.githubusercontent.com/LiveSplit/LiveSplit.github.io/artifacts/LiveSplitDevBuild.zip -OutFile .deps/LiveSplitDevBuild.zip
Expand-Archive .deps/LiveSplitDevBuild.zip -DestinationPath .deps/LiveSplitDevBuild -Force
dotnet build src/LiveSplit.phyhey.ClickControls/LiveSplit.phyhey.ClickControls.csproj -c Release
```

既存の現行LiveSplitを参照する場合は、最後のコマンドに
`-p:LiveSplitPath="C:\path\to\LiveSplit"`を追加します。
参照先には`LiveSplit.Core.dll`と`UpdateManager.dll`が必要です。
ホストのDLLは出力先へコピーしません。

生成物: `src/LiveSplit.phyhey.ClickControls/bin/Release/net481/LiveSplit.phyhey.ClickControls.dll`

## インストール

1. LiveSplitを終了します。
2. 生成された`LiveSplit.phyhey.ClickControls.dll`を、`LiveSplit.exe`と同じフォルダーにある`Components`へコピーします。
3. LiveSplitを起動し、**Edit Layout → + → Control → Click Controls**で追加します。

配布するのはComponent DLLのみです。LiveSplit本体の参照DLLを置き換える必要はありません。

## 検証

上記のビルド・展開後、以下で画面を表示しないスモークテストを実行できます。
テスト用のLiveSplitフォルダーを使用してください。

```powershell
dotnet build tests/SmokeTest/SmokeTest.csproj -c Release
Copy-Item src/LiveSplit.phyhey.ClickControls/bin/Release/net481/LiveSplit.phyhey.ClickControls.dll .deps/LiveSplitDevBuild/Components/
Copy-Item tests/SmokeTest/bin/Release/net481/SmokeTest.exe .deps/LiveSplitDevBuild/
Copy-Item .deps/LiveSplitDevBuild/LiveSplit.exe.config .deps/LiveSplitDevBuild/SmokeTest.exe.config
& ./.deps/LiveSplitDevBuild/SmokeTest.exe
```

LiveSplit自身の`ComponentManager`によるDLL検出・Factory生成、3つの実Button、
Clickイベント経由のStart/Split/Undo/Resetと状態イベント、Paused/Endedの挙動、
設定XML、縦横レイアウトへのControl配置、破棄を検証します。
非表示Formでは`PerformClick()`が動作しないため、テストは`OnClick`を呼んでイベントを発火します。
実際のLiveSplit画面でのマウス操作、見た目、DPI倍率別の表示は未確認です。

## 実装の参照元

2026-09-09に、古いチュートリアルではなく以下の現行ソースを確認しました。

- [LiveSplit upstream](https://github.com/LiveSplit/LiveSplit/tree/5b4aa377ab75bc092331313d197034d5a1dd2fef):
  `IComponentFactory`、`ComponentFactoryAttribute`、`ControlComponent`、`TimerModel`、`LiveSplitState`、`TimerPhase`、Componentローダー。
- [公式Manual Game Time Component](https://github.com/LiveSplit/LiveSplit.ManualGameTime/tree/77b722ff26fe5bfa9df41f3f648ec4f791d78b65):
  SDK形式、net4.8.1、Core/UpdateManager参照、assembly属性、Controlカテゴリの慣習。
- [公式ダウンロードページ](https://livesplit.org/downloads/)のDevelopment Buildをビルド参照・スモークテストに使用。
  検証した`LiveSplit.Core.dll`のFileVersionは`1.8.94.0`です。
  ZIPのSHA-256は`52343EC9C9A241F3489FA5DBF80185F8FC957682FFED58943CC109652495B1EA`です。
  ダウンロードURLの内容は更新されます。

構造は`ClickControlsFactory`（登録）、`ClickControlsComponent`（`ControlComponent`継承、サイズ・必須設定API）、
`ClickControlsControl`（TableLayoutPanel、3つのButton、TimerModel操作）の3クラスです。
Controlのホストへの追加・配置・破棄はLiveSplitの基底クラスが担当します。
