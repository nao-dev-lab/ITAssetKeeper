# IT Asset Keeper – IT資産・機器管理システム
※本リポジトリはポートフォリオ用途として公開しています  

IT Asset Keeper は、社内の IT 機器 (PC・サーバー・NW機器 等) の情報を一元管理し、
変更履歴を追跡しやすくすることを目的とした ASP.NET Core MVC (.NET 8) ベースの Web アプリケーションです。
  
ロール制御（Admin / Editor / Viewer）による権限管理、
機器の CRUD、更新履歴の自動生成、Dashboard 表示に加え、
Azure App Service + Azure SQL へのデプロイまで一連の構築を行っています。  
  
## スクリーンショット (代表）
|ダッシュボード|情報変更履歴 一覧|
|---|---|
|<img width="403.6" height="392.4" alt="dashboard" src="https://github.com/user-attachments/assets/2eb897cb-0f6d-4f48-8db3-5af770195ab8" />|<img width="403.6" height="392.4" alt="スクリーンショット 2025-12-01 123409" src="https://github.com/user-attachments/assets/f26a1ddb-c6ce-46da-9ba7-958971f41e6c" />

***

## 1. 概要 (Overview)
本システムは、社内 IT 資産を一元管理するための Web アプリケーションです。  
機器の登録・編集・廃棄などの状態管理に加え、すべての更新履歴を自動的に記録することで
「いつ・誰が・何を変更したのか」を追跡できるようになっています。

**主な特徴：**
- シンプルな CRUD 画面(Bootstrap + カスタムCSS)
- 検索・フィルタリング（カテゴリ、用途、状態、フリーワード）
- 更新履歴の Before / After 表示
- ロール制御による機能アクセス制限（Admin / Editor / Viewer）
- Dashboard（Chart.js）での集計可視化
- Azure App Service + Azure SQL によるクラウド動作

***

## 2. 主な機能 (Features)
**🔶 認証・認可（Identity）**
- ASP.NET Core Identity を利用
- ロール：Admin / Editor / Viewer

**🔶 機器管理（CRUD）**
- 登録 / 編集 / 詳細 / 削除
- 項目ごとの入力チェック

**🔶 更新履歴（DeviceHistory）**
- 削除 / 新規登録 / 更新 時に自動で Before / After を記録
- 変更箇所をハイライトで表示

**🔶 検索・フィルタリング**
- フリーワード、詳細検索
- URLクエリで状態保持

**🔶 Dashboard**
- Chart.js による可視化グラフ
- 状態別の台数集計など

**🔶 クラウドデプロイ**
- Azure App Service
- Azure SQL
- Cold Start 対策としての loading index.html

**🔶 エラー処理**
- カスタム 400番台/ 500 ページ
- TempDataによるメッセージ通知

***

## 3. 技術スタック (Tech Stack)
**🟪 Backend**
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- ASP.NET Core Identity
- Serilog

**🟪 Frontend**
- Bootstrap 5
- カスタムCSS
- Chart.js
- JavaScript

**🟪 Database**
- Azure SQL
- EF Core Migrations

**🟪 Infrastructure / Cloud**
- Azure App Service (Free)
- Azure SQL Database (Free)

***

## 4. アーキテクチャ (Architecture)
**🔷 ディレクトリ構成（抜粋）**
```
ITAssetKeeper/
 ├─ Controllers/
 ├─ Models/
 │   ├─ Entities/
 │   ├─ Enums/
 │   ├─ ViewModels/
 │   └─ DTO/
 ├─ Services/
 ├─ Views/
 ├─ Migrations/
 └─ wwwroot/
```

**🔷 レイヤード構造**
- Entity：DBテーブルと1対1
- DTO：画面表示用の集約データ
- ViewModel：画面ごとの入力専用モデル
- Service：ビジネスロジックを集約

**🔷 ER図**  
<img width="476" height="457.8" alt="er" src="https://github.com/user-attachments/assets/5c88c227-8bd6-435d-b6dc-4d2b6723e85d" />


***

## 5. 主要画面のスクリーンショット (Screenshots)

|Login|Dashboard|
|---|---|
|<img width="376.4" height="226" alt="login" src="https://github.com/user-attachments/assets/6950386d-f022-4be9-9f81-b885a7231584" />|<img width="403.6" height="392.4" alt="dashboard" src="https://github.com/user-attachments/assets/2eb897cb-0f6d-4f48-8db3-5af770195ab8" />|

|Device 一覧|Device 詳細検索|
|---|---|
|<img width="403.6" height="389.6" alt="deviceList1" src="https://github.com/user-attachments/assets/9c2a19b9-96e3-462e-9a2b-e71342e05c3a" />|<img width="403.6" height="389.6" alt="deviceList2" src="https://github.com/user-attachments/assets/185d04bd-34a2-4345-9dbf-329556ebd9ff" />|

|Device 新規登録|Device 詳細|
|---|---|
|<img width="412.8" height="310.8" alt="deviceCreate" src="https://github.com/user-attachments/assets/b694f497-ec93-4053-bee3-841a517c15d6" />|<img width="403.6" height="392.4" alt="deviceDetail" src="https://github.com/user-attachments/assets/b02727c3-89fd-451c-9afa-6d2795338b1c" />|

|履歴一覧|履歴詳細（Before/After）|
|---|---|
|<img width="403.6" height="392.4" alt="historyList" src="https://github.com/user-attachments/assets/f26a1ddb-c6ce-46da-9ba7-958971f41e6c" />|<img width="403.6" height="416.8" alt="deviceHistoryDetail" src="https://github.com/user-attachments/assets/81bc9507-4f30-453f-979c-e2abc7e89a83" />|

***

## 6. 機能詳細 (Detailed Features)
**🔶 Device CRUD**
- 入力検証(ModelState)と統一エラーメッセージ
- 一覧の省略表示(hoverで全表示)
- ソフトデリート方式：削除した機器は一覧から非表示になり、履歴のみ保持

**🔶 DeviceHistory**
- Before / After の自動生成
- 取り消し線つきの差分表示
- Enum による ChangeType（登録/削除/更新） 管理
- 削除操作は ChangeType=Deleted として記録

**🔶 検索機能**
- 複数条件検索（カテゴリ/用途/状態/キーワード）
- 入力保持（URLクエリ方式）
- SortKeys / SortOrders の Enum 化
- ページネーション対応（今後拡張予定）

**🔶 ロール管理**
- Admin：全機能使用可能、全機器表示可能
- Editor：一部項目のみ編集可能、登録/削除不可、インフラ機器は非表示
- Viewer：閲覧のみ、インフラ機器は非表示
- UI・Controller 双方向での権限チェック

**🔶 例外処理 / ログ**
- try-catch による例外捕捉とカスタムエラーページ
- Serilog による詳細ログ（スタックトレース・ModelState エラー）

**🔶 Utility / Helper**
- EEnumDisplay名変換 (EnumDisplayHelper)
- 履歴表示のためのカスタム変換ロジック
- 共通処理 (文字列整形・日付変換等)の集約

***

## 7. 使い方（Usage / How to use）
このアプリケーションは、デモ環境として公開しています。  
機器管理や変更履歴など、主要な機能を 管理者権限 で自由にお試しいただけます。  

1. 下記のデモ URL にアクセスします。  
2. デモアカウントでログインします。  
3. 機器一覧 → 詳細 → 編集 → 履歴確認、削除 など、管理フローを一通り体験できます。  
※ 削除した機器はソフトデリートで管理され、一覧からは非表示になりますが、履歴には残ります。
  
※ 本デモ環境には個人情報・実データは含まれていません。  
※ 表示される人物名や機器情報はすべて架空データであり、実在の人物・企業とは関係ありません。  

***

## 8. デモ URL / デモアカウント (Demo URL / Demo Accounts)
**🔷 公開中のアプリケーション（Azure App Service）**  
URL：https://itassetkeeper20251201105549-gpa2f7a8fgcpd6gr.eastasia-01.azurewebsites.net
- 無料プラン(F1)で公開しているため、アクセス時にアプリの起動で数秒ほどお時間をいただく場合があります。
- アプリの挙動確認のため、デモ用データは定期的に初期状態へリフレッシュされます。

**🔷 デモアカウント（管理者権限）**  
- ユーザー名： demo-admin
- パスワード： DemoAdmin123!  
※ デモ専用アカウントです。自由に編集・削除などお試しいただけます。  
※ データは適宜リセットされる場合があります。  

***

## 9. 開発背景・目的 (Background)
Web アプリケーション開発の学習のため、設計から実装・デプロイまでを一通り行い構築した IT 資産管理システムです。
開発期間：2025年11月〜2025年12月（約4週間）

**🔶 実務で感じた課題**  
前職では、PC・周辺機器の管理において以下のような問題が発生していました：
- 誰がどの機器を使用しているのか分からない
- どこに設置されているか把握しづらい
- 貸与・返却の履歴が残らない
- 廃棄予定や在庫数の判断が困難
- 購入時期が不明で更新計画が立てにくい  
これらの非効率を改善するため、
「状態が直感的に分かる UI」「変更履歴の追跡」「権限に応じた操作制御」を重視して設計しました。

**🔶 設計時に意識したこと**
- 状態が分かりやすい UI
- 更新履歴の記録方法（Before/After）の最適化
- 変更箇所の視覚的なわかりやすさ
- ロール別の操作制御による安全な運用

**🔶 学開発を通じて学んだこと**
- Webアプリケーション特有の MVC 構造の理解
- EF Core によるデータアクセスと Migration 運用
- Services・Helper による責務分離
- Identity を用いた認証・認可設計
- Azure App Service + Azure SQL のデプロイ・運用
- ユーザー体験を考慮した UI / エラー処理 / ローディング画面

***

## 8. ローカル実行方法（Setup）
**1. .NET 8 のインストール**  
https://dotnet.microsoft.com/

**2. DB 接続設定**  
appsettings.Development.json に接続文字列を設定：
```
"ConnectionStrings": {
  "DefaultConnection": "Server=...;Database=...;User ID=...;Password=...;"
}
```

**3. マイグレーション**
```
dotnet ef database update
```

**4.起動**
```
dotnet run
```

***

## 9. デプロイ構成 (Deployment)
**🟪 Azure App Service**
- Publish (WebDeploy)
- フレームワーク依存デプロイ
- index.html による Cold Start 対策

**🟪 Azure SQL**
- 無料枠（32GB）
- ファイアウォール：自PC + Azureサービスのみ

**🟪 App Service の環境変数**
```
WEBSITE_TIME_ZONE = Tokyo Standard Time
```

***

## 10. 今後の拡張 (Future Plans)
|機能|内容|
|---|---|
|AutoMapper の導入|DTO / Entity 変換の自動化による責務分離の強化。|
|ユーザー管理機能|ユーザー追加・権限変更・利用状況の管理など。|
|ページサイズ変更機能|一覧画面で 10 / 25 / 50 件などの表示切替を可能にする。|
|検索条件の保持|(一覧→詳細→戻る)操作で検索条件が保持される導線改善。|
|パスワードリセット機能|メールリンク等によるパスワード再設定フロー。|
|設置場所マップ|フロア図上で機器の配置を視覚的に管理。|
|マップからの機器選択・登録|設置場所マップ上から直接機器登録・移設を行う導線。|
|原価償却管理|取得価格・耐用年数に基づく償却費・簿価の自動計算。|
|リース / レンタル契約管理|契約開始/終了、満了通知、買取可否などの契約情報を管理。|

***
