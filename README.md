<div align="center">

# 🎧 SONARA

### Sanatçı Odaklı, Çok Katmanlı (Tiered) Üyelik ve SaaS Yönetim Mimarisine Sahip Dijital Müzik Platformu

<p>
  <img src="https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/Azure-Blob%20Storage-0089D6?style=for-the-badge&logo=microsoftazure&logoColor=white" />
  <img src="https://img.shields.io/badge/Hangfire-Background%20Jobs-8A2BE2?style=for-the-badge" />
  <img src="https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" />
  <img src="https://img.shields.io/badge/SQL%20Server-2022-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
</p>

<p>
  <a href="#-neden-sonara">Neden Sonara?</a> •
  <a href="#-sistem-mimarisi">Sistem Mimarisi</a> •
  <a href="#-teknik-derinlik--özellikler">Teknik Derinlik</a> •
  <a href="#-ekran-görüntüleri">Ekran Görüntüleri</a> •
  <a href="#-teknoloji-yığını">Teknoloji Yığını</a> •
  <a href="#-kurulum-ve-yapılandırma">Kurulum</a>
</p>

<br>

> Modern SaaS mimarisi, gelişmiş güvenlik yaklaşımı ve bulut tabanlı medya yönetimi ile geliştirilen dijital müzik platformu.

<br>

<img src="https://github.com/user-attachments/assets/b5768da8-64d5-4829-885d-227504ef87ac" width="100%" />

</div>

---

# 💡 Neden Sonara?

Piyasadaki birçok müzik platformu örneği; yalnızca temel CRUD işlemlerini gerçekleştiren, basit kimlik doğrulaması kullanan ve gerçek üretim ortamlarında karşılaşılan ihtiyaçları göz ardı eden demo projelerden oluşmaktadır.

**Sonara**, yalnızca bir müzik dinleme uygulaması değil; gerçek bir SaaS platformunun karşılaşacağı güvenlik, üyelik yönetimi, medya depolama ve oturum kontrolü problemlerini çözmek amacıyla geliştirildi.

## ✨ Öne Çıkan Özellikler

| | |
|:--|:--|
| 🛡️ **Stateful Access Control** | JWT claim'lerine güvenmek yerine kritik isteklerde kullanıcının üyelik durumu, rolü ve aktifliği veritabanından yeniden doğrulanır. |
| 🔄 **Otonom Arka Plan Görevleri** | Hangfire ile süresi dolan Premium üyelikler otomatik olarak **Free** seviyesine düşürülür. |
| 📱 **Çoklu Cihaz Yönetimi** | Üyelik paketine göre cihaz limiti uygulanır. Limit aşıldığında en eski oturum (FIFO) otomatik sonlandırılır. |
| ☁️ **Bulut Tabanlı Medya Yönetimi** | Şarkılar ve görseller Azure Blob Storage üzerinde saklanır ve CDN uyumlu çalışır. |
| 🎨 **Modern Yönetim Paneli** | Hazır admin template'i kullanılmadan, Vercel & Linear tasarım anlayışıyla geliştirildi. |
| 🚀 **Üretim Odaklı Mimari** | Katmanlı mimari, Repository Pattern ve API tabanlı iletişim sayesinde ölçeklenebilir yapı sunar. |

---

# 🏗️ Sistem Mimarisi

> Sonara, kullanıcı arayüzü ile iş mantığını birbirinden tamamen ayıran **2-Tier Proxy & Repository Pattern** mimarisine sahiptir.

- **Sonara.WebUI** katmanı veritabanına **doğrudan erişmez**.
- Tüm istekler güvenli **SonaraApiClient** üzerinden **Sonara.WebApi**'ye iletilir.
- Veri erişimi Repository katmanı üzerinden gerçekleştirilir.
- İş kuralları servis katmanında uygulanır.
- Kimlik doğrulama JWT ve ASP.NET Core Identity ile yönetilir.

---

                               ┌─────────────────────────┐
                               │   Sonara.WebUI (MVC)    │
                               │  (Razor Views + ES6 JS) │
                               └────────────┬────────────┘
                                            │ HTTP / REST API (HttpOnly Cookie JWT)
                                            ▼
                               ┌─────────────────────────┐
                               │  Sonara.WebApi (REST)   │
                               │  Identity + Guard Auth  │
                               └┬───────────┬───────────┬┘
                                │           │           │
       ┌────────────────────────┘           │           └────────────────────────┐
       ▼                                    ▼                                    ▼
┌──────────────┐                  ┌──────────────────┐                 ┌──────────────────┐
│  EF Core 10  │                  │  Hangfire Engine │                 │ Azure Blob Engine│
│ Repository   │                  │  (Cron: 03:00)   │                 │ (Audio Stream)   │
└──────┬───────┘                  └─────────┬────────┘                 └──────────────────┘
       │                                    │
       └──────────────────┬─────────────────┘
                          ▼
            ┌───────────────────────────┐
            │   SQL Server Database     │
            └───────────────────────────┘

# 🗂️ Proje Dizin Yapısı

```text
Sonara.sln
├── 📁 Sonara.CoreLayer
│   └── Domain entity'leri (Song, Artist, Playlist, MembershipPlan vb.)

├── 📁 Sonara.DataAccessLayer
│   └── DbContext, Entity Configurations, Migrations ve Repository Implementations

├── 📁 Sonara.DtoLayer
│   └── Request / Response DTO kontratları

├── 📁 Sonara.WebApi
│   └── REST API, JWT Authentication, Identity Services ve Hangfire Jobs

└── 📁 Sonara.WebUI
    └── ASP.NET Core MVC, Razor Views, API Client Proxy ve Custom CSS Design System
```

---

# ⚡ Teknik Derinlik & Özellikler

## 🎧 Dinleyici Deneyimi

| Özellik | Açıklama |
|---------|----------|
| 🎵 **Kesintisiz Ses Akışı** | HTML5 Audio API tabanlı oynatıcı ile cihazlar arasında kaldığı yerden devam eden müzik deneyimi. |
| 🏷️ **Dinamik Metadata** | MP3 dosyalarının süre, bitrate ve kapak bilgileri TagLibSharp kullanılarak otomatik okunur. |
| 🎯 **Ruh Haline Göre Keşif** | Enerji, Odaklan, Yol Modu, Yağmurlu gibi kategorilerle içerik keşfi. |
| 🔒 **IDOR Koruması** | Playlist işlemlerinde Ownership Guard doğrulaması sayesinde yalnızca sahibi düzenleme yapabilir. |
| 🛡️ **Brute Force Koruması** | 5 başarısız giriş denemesinden sonra hesap 10 dakika boyunca kilitlenir. |

---

## 💳 Tiered Membership

| Özellik | Açıklama |
|---------|----------|
| 👑 **4 Katmanlı Üyelik** | **Free → Basic → Gold → Elite** üyelik sistemi. |
| 🚀 **Dinamik Yetkilendirme** | Yetkisi olmayan içeriklerde otomatik Premium yükseltme yönlendirmesi. |

---

## 👨‍💼 SaaS Admin Paneli

| Özellik | Açıklama |
|---------|----------|
| 📊 **Gerçek Zamanlı Dashboard** | Katalog dağılımları ve dinleme istatistiklerini gösteren yönetim paneli. |
| ⚙️ **Tam Kapsamlı CRUD** | Şarkı, Sanatçı, Albüm ve Üyelik Paketleri yönetimi. |
| ☁️ **Asenkron Medya Yükleme** | Azure Blob Storage destekli sürükle-bırak dosya yükleme sistemi. |
| 🔐 **Rol Bazlı Yetkilendirme** | `[Authorize(Roles = "Admin")]` tabanlı erişim kontrolü. |

---

## 📸 Ekran Görüntüleri

### 🔐 Giriş & Kayıt Sayfası • 🏠 Keşfet (Dashboard)

<p align="center">
  <img src="https://github.com/user-attachments/assets/e79cd1bc-7c55-4b39-a927-6526ac07682e" width="49%" />
  <img src="https://github.com/user-attachments/assets/08bee1dc-e2b3-4261-a397-36f0146fa219" width="49%" />
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/8b7f8fcc-e926-4521-a784-b2b6326a1264" width="100%" />
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/e7c7ab0b-8447-4166-929c-defdb1371a9e" width="49%" />
  <img src="https://github.com/user-attachments/assets/314cfe9d-d900-4e7d-a288-dc199925dfd9" width="49%" />
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/f96481f7-edca-4c52-8d0b-b1cc9eccb8e6" width="49%" />
  <img src="https://github.com/user-attachments/assets/ea69e78d-bbde-4db0-abec-69cbf46f2bf5" width="49%" />
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/58c75f63-8ba9-4650-ae3a-802dae7b2b24" width="100%" />
</p>

---
### 🎤 Sanatçı Detay • 🎵 Playlist Detay

<p align="center">
  <img src="https://github.com/user-attachments/assets/03c2a531-4cee-47a6-80ec-cf06364ce097" width="49%" />
  <img src="https://github.com/user-attachments/assets/7eea8aac-a7a7-465e-84cc-9d2bc552e73e" width="49%" />
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/d738eaf8-2a40-4ade-87b4-d6f3505c8a72" width="100%" />
</p>

---

### 🎵 Tüm Şarkılar • 💎 Paket Detayı

<p align="center">
  <img src="https://github.com/user-attachments/assets/a089b69a-7807-4e05-9cec-bf305ebeeb8c" width="49%" />
  <img src="https://github.com/user-attachments/assets/f63db565-e2a6-4e3f-8150-21f5abb7be42" width="49%" />
</p>

---



### 📊 Admin — Genel Bakış

<p align="center">
  <img src="https://github.com/user-attachments/assets/838a5877-4de2-42f2-a512-38d72f0a21b8" width="100%" />
</p>

---


### 🛠️ Admin — Şarkı Yönetimi • Admin — Sanatçı Yönetimi • Admin — Mod Yönetimi • Admin — Paket Yönetimi

<p align="center">
  <img src="https://github.com/user-attachments/assets/c31d59ff-8e49-4a3d-bc68-ed69b9be4651" width="49%" />
  <img src="https://github.com/user-attachments/assets/67e05433-3fb7-44ca-b859-b9e1065ad018" width="49%" />
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/41232a56-5116-428e-b2a7-9a880c454640" width="49%" />
  <img src="https://github.com/user-attachments/assets/0eabe88b-cb4d-4456-89d6-0f02663b08d8" width="49%" />
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/afb57613-b5e7-4d5b-87ca-98d02a60bf3c" width="49%" />
  <img src="https://github.com/user-attachments/assets/ff7de322-992e-43b8-a7d9-7288d0df2a93" width="49%" />
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/1132417e-aa22-4ce1-b577-7a42130cb52d" width="49%" />
  <img src="https://github.com/user-attachments/assets/9d910114-bb00-41c4-8eb5-3994098d3d7f" width="49%" />
</p>

---



# 🛠️ Teknoloji Yığını

## ⚙️ Backend & Sunucu

| Teknoloji | Açıklama |
|-----------|----------|
| **Framework** | ASP.NET Core 10 Web API |
| **ORM** | Entity Framework Core 10 |
| **Veritabanı** | SQL Server |
| **Kimlik Doğrulama** | ASP.NET Core Identity + JWT Bearer (HttpOnly Cookie) |
| **Arka Plan Görevleri** | Hangfire |
| **Bulut Depolama** | Azure Blob Storage |
| **Medya Metadata** | TagLibSharp |
| **Object Mapping** | AutoMapper |

---

## 🎨 Frontend & UI

| Teknoloji | Açıklama |
|-----------|----------|
| **Mimari** | ASP.NET Core MVC (Razor Views) |
| **JavaScript** | Vanilla JavaScript (ES6+ Modules) |
| **Styling** | Custom CSS Design System (Aero / Linear Dark Minimalist) |

---

# 🚀 Kurulum ve Yapılandırma

## 📋 Ön Gereksinimler

- .NET 10 SDK
- SQL Server 2019+ veya LocalDB
- Azure Storage Account *(veya Azurite Emulator)*

---

## 1️⃣ Repoyu Klonlayın

```bash
git clone https://github.com/dilanderegozu/Sonara.git
cd Sonara
```

---

## 2️⃣ User Secrets Yapılandırması

> **Sonara.WebApi** projesinde çalıştırın.

```bash
cd Sonara.WebApi

dotnet user-secrets set "AzureStorage:ConnectionString" "<YOUR_AZURE_CONNECTION_STRING>"

dotnet user-secrets set "JwtSettings:Secret" "<YOUR_LONG_JWT_SECRET_KEY>"
```

---

## 3️⃣ Veritabanını Oluşturun

```bash
dotnet ef database update --project ../Sonara.DataAccessLayer/Sonara.DataAccessLayer.csproj
```

---

## 4️⃣ Uygulamaları Başlatın

### Terminal 1 — Web API

```bash
cd Sonara.WebApi

dotnet run
```

### Terminal 2 — Web UI

```bash
cd Sonara.WebUI

dotnet run --launch-profile https
```

---

## 5️⃣ Tarayıcıda Açın

```text
https://localhost:7113
```

---

# 🗺️ Yol Haritası

- [ ] Google & Spotify OAuth 2.0 ile Sosyal Giriş
- [ ] SignalR ile Gerçek Zamanlı Admin Dashboard Metrikleri
- [ ] CQRS & MediatR Mimarisi
- [ ] Redis Distributed Cache Entegrasyonu

---

<div align="center">

### ⭐ Sonara

Modern SaaS mimarisi, gelişmiş güvenlik ve bulut tabanlı medya yönetimiyle geliştirilen dijital müzik platformu.

**Designed & Developed with ❤️ by Dilan Deregözü**

</div>
