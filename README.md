<div align="center">

  <h1>🎧 SONARA</h1>
  <p><strong>Sanatçı Odaklı, Çok Katmanlı (Tiered) Üyelik ve SaaS Yönetim Mimarisine Sahip Dijital Müzik Platformu</strong></p>

  <p>
    <a href="https://dotnet.microsoft.com/"><img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10" /></a>
    <a href="https://azure.microsoft.com/"><img src="https://img.shields.io/badge/Azure-Blob_Storage-0089D6?style=for-the-badge&logo=microsoftazure&logoColor=white" alt="Azure" /></a>
    <a href="https://www.hangfire.io/"><img src="https://img.shields.io/badge/Hangfire-Job_Scheduler-8A2BE2?style=for-the-badge" alt="Hangfire" /></a>
    <a href="https://jwt.io/"><img src="https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" alt="JWT" /></a>
    <a href="https://www.microsoft.com/sql-server"><img src="https://img.shields.io/badge/SQL_Server-2022-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" /></a>
  </p>

  <p>
    <a href="#-neden-sonara">Neden Sonara?</a> •
    <a href="#-sistem-mimarisi">Sistem Mimarisi</a> •
    <a href="#-teknik-derinlik--özellikler">Teknik Derinlik</a> •
    <a href="#-ekran-görüntüleri">Ekran Görüntüleri</a> •
    <a href="#-teknoloji-yığını">Teknoloji Yığını</a> •
    <a href="#-kurulum-ve-yapılandırma">Kurulum</a>
  </p>

  <br />

  <img src="docs/screenshots/welcome.png" alt="Sonara Platform Showcase" width="100%" style="border-radius: 10px; box-shadow: 0 10px 30px rgba(0,0,0,0.3);" />

</div>

---

## 💡 Neden Sonara?

Piyasadaki tipik "müzik uygulaması" demoları genellikle tek katmanda CRUD işlemleri yapan, oturum yönetimini yüzeysel ele alan ve gerçek dünya prodüksiyon ihtiyaçlarını göz ardı eden yapılardır. 

**Sonara**, bir müzik yayın platformunun arkasındaki **gerçek SaaS operasyonlarını ve güvenlik zorluklarını** çözmek üzere sıfırdan tasarlandı:

- 🛡️ **Stateful Access Control:** Yalnızca JWT Claim'lerine güvenmek yerine, her hassas istekte veritabanı seviyesinde anlık yetki ve aktiflik doğrulaması.
- 🔄 **Otonom Arka Plan Görevleri (Hangfire):** Süresi dolan üyeliklerin gece senkronizasyonuyla otomatik olarak `Free` statüsüne düşürülmesi.
- 📱 **Eşzamanlı Oturum & Cihaz Limiti:** Kota dolduğunda FIFO (First-In-First-Out) mantığıyla en eski oturumu ve JWT Refresh Token'ı otomatik olarak düşüren oturum yönetimi.
- ☁️ **Bulut Yerel Medya Yönetimi:** Şarkı ve görsellerin sunucu lokalinde (`wwwroot`) değil, üretim standartlarına uygun olarak **Azure Blob Storage** üzerinde CDN uyumlu tutulması.
- 🎨 **SaaS Standartlarında Admin Paneli:** Şablon kullanılmadan, Vercel/Linear tasarım dilinden ilham alınarak sıfırdan yazılmış modüler yönetim arayüzü.

---

## 🏗️ Sistem Mimarisi

Sonara, kullanıcı arayüzü ile iş mantığını birbirinden tamamen ayıran **2-Tier Proxy & Repository Pattern** mimarisine sahiptir. `Sonara.WebUI` katmanı veritabanına asla doğrudan erişmez; tüm veri alışverişi güvenli `SonaraApiClient` aracılığıyla `Sonara.WebApi` üzerinden yürütülür.

```
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
```

### 🗂️ Proje Dizin Yapısı

```bash
Sonara.sln
├── 📁 Sonara.CoreLayer         # Domain Entity'leri (Song, Artist, Playlist, MembershipPlan vb.)
├── 📁 Sonara.DataAccessLayer   # DbContext, Entity Configurations, Migrations & Repository Impl.
├── 📁 Sonara.DtoLayer          # Strict Request/Response DTO Kontratları
├── 📁 Sonara.WebApi            # REST API, JWT Middleware, Identity Services & Hangfire Jobs
└── 📁 Sonara.WebUI             # MVC Katmanı, Custom CSS Design System & API Client Proxy
```

---

## ⚡ Teknik Derinlik & Özellikler

### 🎧 Dinleyici & Kullanıcı Deneyimi
- **Kesintisiz Ses Akışı:** Cihazlar arası kaldığı yerden devam edebilen, HTML5 Audio API tabanlı ses oynatıcısı.
- **Dinamik Metadata Ayıklama:** Yüklenen MP3 dosyalarının süresi, bitrate ve kapak bilgileri `TagLibSharp` ile sunucu tarafında otomatize olarak okunur.
- **Ruh Haline Göre Keşif:** Odaklan, Enerji, Yağmurlu, Yol Modu gibi parametrik ruh hali filtreleme altyapısı.
- **IDOR Korunmalı Playlist Yönetimi:** Sadece ilgili kullanıcının müdahale edebildiği sahiplik doğrulama (Ownership Guard) altyapısı.
- **Brute-Force Koruması:** 5 hatalı giriş denemesinde hesabı 10 dakika otomatik kilitleyen güvenlik politikası.

### 💳 Tiered Membership (Çok Katmanlı Üyelik)
- **4 Seviyeli Erişim Katmanı:** `Free` → `Basic` → `Gold` → `Elite`
- **Anlık Erişim Kısıtlaması:** Yetki kapsamı dışındaki içeriklerde otomatik "Yükselt (Upgrade)" yönlendirme mekanizması.

### 👑 SaaS Admin Paneli
- **Gerçek Zamanlı Analitik:** Katalog dağılımları ve dinleme istatistiklerini içeren dinamik dashboard.
- **Tam Kapsamlı CRUD:** Şarkı, Sanatçı, Album ve Paket yönetimi.
- **Sürükle-Brak Medya Yükleyici:** Görsel ve ses dosyaları için asenkron bulut yükleme arayüzü.
- **Rol Bazlı Güvenlik:** `[Authorize(Roles = "Admin")]` özniteliği ile tam yetki ayrıştırması.

---

## 📸 Ekran Görüntüleri

| Giriş & Kayıt Sayfası | Keşfet (Dashboard) |
| :---: | :---: |
| <img src="docs/screenshots/login.png" width="400" alt="Login"> | <img src="docs/screenshots/dashboard.png" width="400" alt="Dashboard"> |

| Sanatçı Detay | Playlist Detay |
| :---: | :---: |
| <img src="docs/screenshots/artist-detail.png" width="400" alt="Artist Detail"> | <img src="docs/screenshots/playlist-detail.png" width="400" alt="Playlist Detail"> |

| Üyelik Paketleri | Admin — Genel Bakış |
| :---: | :---: |
| <img src="docs/screenshots/plans.png" width="400" alt="Plans"> | <img src="docs/screenshots/admin-overview.png" width="400" alt="Admin Overview"> |

| Admin — Şarkı Yönetimi | Admin — Sanatçı Yönetimi |
| :---: | :---: |
| <img src="docs/screenshots/admin-songs.png" width="400" alt="Admin Songs"> | <img src="docs/screenshots/admin-artists.png" width="400" alt="Admin Artists"> |

---

## 🛠️ Teknoloji Yığını

### **Backend & Sunucu Katmanı**
- **Framework:** ASP.NET Core 10 Web API
- **ORM & DB:** Entity Framework Core 10, SQL Server
- **Kimlik Doğrulama:** ASP.NET Core Identity, JWT Bearer Tokens (HttpOnly Cookie Transport)
- **Arka Plan Görevleri:** Hangfire
- **Bulut Depolama:** Azure Blob Storage SDK
- **Medya Metadata:** TagLibSharp
- **Object Mapper:** AutoMapper

### **Frontend & Arayüz Katmanı**
- **Mimari:** ASP.NET Core MVC (Razor Views)
- **Scripting:** Pure Vanilla JS (ES6+ Native Modüller)
- **Styling:** Custom CSS Design System (Aero/Linear Dark Minimalist Aesthetic)

---

## 🚀 Kurulum ve Yapılandırma

### Ön Gereksinimler
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server 2019+ / LocalDB
- Azure Storage Account (veya Azurite Emulator)

### Adım Adım Çalıştırma

1. **Repoyu Klonlayın:**
   ```bash
   git clone https://github.com/dilanderegozu/Sonara.git
   cd Sonara
   ```

2. **User Secrets Yapılandırması (WebApi projesinde):**
   ```bash
   cd Sonara.WebApi
   dotnet user-secrets set "AzureStorage:ConnectionString" "<YOUR_AZURE_CONNECTION_STRING>"
   dotnet user-secrets set "JwtSettings:Secret" "<YOUR_LONG_JWT_SECRET_KEY>"
   ```

3. **Veritabanı Migration'larını Uygulayın:**
   ```bash
   dotnet ef database update --project ../Sonara.DataAccessLayer/Sonara.DataAccessLayer.csproj
   ```

4. **Uygulamaları Başlatın:**
   ```bash
   # Terminal 1 - API Servisi
   cd Sonara.WebApi
   dotnet run

   # Terminal 2 - UI Servisi
   cd Sonara.WebUI
   dotnet run --launch-profile https
   ```

5. **Tarayıcıda Açın:**  
   `https://localhost:7113`

---

## 🗺️ Yol Haritası (Roadmap)

- [ ] Google / Spotify OAuth 2.0 ile Sosyal Giriş Entegrasyonu
- [ ] SignalR ile Admin Dashboard üzerinde Anlık Dinleyici Sayısı / Canlı Akış Metrikleri
- [ ] CQRS & MediatR Deseni ile Admin Katmanının Yeniden Yapılandırılması
- [ ] Redis Distributed Caching ile Sık Dinlenen Şarkı Metadatalarının Önbelleğe Alınması

---

<div align="center">
  <p>Designed & Developed with ❤️ by <strong>Dilan Deregözü</strong></p>
</div>
