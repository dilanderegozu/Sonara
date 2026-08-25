<div align="center">

# 🎧 SONARA

### Sanatçı Odaklı, Çok Katmanlı (Tiered) Üyelik ve SaaS Yönetim Mimarisine Sahip Dijital Müzik Platformu

<p>
  <img src="https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/Azure-Blob%20Storage-0089D6?style=for-the-badge&logo=microsoftazure&logoColor=white" alt="Azure Blob Storage" />
  <img src="https://img.shields.io/badge/Hangfire-Background%20Jobs-8A2BE2?style=for-the-badge" alt="Hangfire" />
  <img src="https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" alt="JWT Authentication" />
  <img src="https://img.shields.io/badge/SQL%20Server-2022-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
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

> **Modern SaaS mimarisi, gelişmiş güvenlik yaklaşımı ve bulut tabanlı medya yönetimi ile geliştirilen dijital müzik platformu.**

<!-- Proje ekran görüntünüzü ekleyin -->
<img src="docs/screenshots/welcome.png" alt="Sonara" width="100%"/>

</div>

---

# 💡 Neden Sonara?

Piyasadaki birçok müzik platformu örneği; yalnızca temel CRUD işlemlerini gerçekleştiren, basit kimlik doğrulaması kullanan ve gerçek üretim ortamlarında karşılaşılan ihtiyaçları göz ardı eden demo projelerden oluşmaktadır.

**Sonara**, yalnızca bir müzik dinleme uygulaması değil; **gerçek bir SaaS platformunun karşılaşacağı güvenlik, üyelik yönetimi, medya depolama ve oturum kontrolü problemlerini çözmek amacıyla** geliştirildi.

## Öne Çıkan Özellikler

### 🛡️ Stateful Access Control
JWT içerisinde bulunan claim'lere güvenmek yerine, kritik isteklerde kullanıcının üyelik durumu, rolü ve aktifliği veritabanından yeniden doğrulanır. Böylece üyelik değişiklikleri anında sisteme yansır.

### 🔄 Otonom Arka Plan Görevleri
Hangfire ile çalışan zamanlanmış görevler sayesinde süresi dolan Premium üyelikler otomatik olarak **Free** üyeliğe düşürülür ve sistem manuel müdahaleye ihtiyaç duymaz.

### 📱 Çoklu Cihaz ve Oturum Yönetimi
Her üyelik paketi belirli sayıda aktif cihaz destekler. Limit aşıldığında sistem **FIFO (First-In First-Out)** mantığıyla en eski oturumu ve Refresh Token'ını otomatik olarak sonlandırır.

### ☁️ Bulut Tabanlı Medya Yönetimi
Şarkılar, albüm kapakları ve sanatçı görselleri uygulama sunucusunda değil, **Azure Blob Storage** üzerinde saklanır. Böylece yüksek ölçeklenebilirlik ve CDN uyumluluğu sağlanır.

### 🎨 Modern Yönetim Paneli
Hazır admin template'leri kullanılmadan, Vercel ve Linear tasarım anlayışından ilham alınarak tamamen özel geliştirilmiş yönetim paneli sunulmaktadır.

### 🚀 Üretim Odaklı Mimari
Katmanlı mimari, Repository Pattern, servis soyutlamaları ve API tabanlı iletişim sayesinde proje kolayca ölçeklenebilir ve sürdürülebilir yapıdadır.

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
