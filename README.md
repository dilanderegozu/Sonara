<div align="center">

🎧 SONARA

Sanatçı Odaklı, Çok Katmanlı (Tiered) Üyelik ve SaaS Yönetim Mimarisine Sahip Dijital Müzik Platformu

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

Modern SaaS mimarisi, gelişmiş güvenlik yaklaşımı ve bulut tabanlı medya yönetimi ile geliştirilen dijital müzik platformu.

<!-- Proje ekran görüntünüzü ekleyin -->

<img width="1897" height="910" alt="Ekran görüntüsü 2026-08-25 125049" src="https://github.com/user-attachments/assets/b5768da8-64d5-4829-885d-227504ef87ac" />

</div>

💡 Neden Sonara?

Piyasadaki birçok müzik platformu örneği; yalnızca temel CRUD işlemlerini gerçekleştiren, basit kimlik doğrulaması kullanan ve gerçek üretim ortamlarında karşılaşılan ihtiyaçları göz ardı eden demo projelerden oluşmaktadır.

Sonara, yalnızca bir müzik dinleme uygulaması değil; gerçek bir SaaS platformunun karşılaşacağı güvenlik, üyelik yönetimi, medya depolama ve oturum kontrolü problemlerini çözmek amacıyla geliştirildi.

Öne Çıkan Özellikler

🛡️ Stateful Access Control

JWT içerisinde bulunan claim'lere güvenmek yerine, kritik isteklerde kullanıcının üyelik durumu, rolü ve aktifliği veritabanından yeniden doğrulanır. Böylece üyelik değişiklikleri anında sisteme yansır.

🔄 Otonom Arka Plan Görevleri

Hangfire ile çalışan zamanlanmış görevler sayesinde süresi dolan Premium üyelikler otomatik olarak Free üyeliğe düşürülür ve sistem manuel müdahaleye ihtiyaç duymaz.

📱 Çoklu Cihaz ve Oturum Yönetimi

Her üyelik paketi belirli sayıda aktif cihaz destekler. Limit aşıldığında sistem FIFO (First-In First-Out) mantığıyla en eski oturumu ve Refresh Token'ını otomatik olarak sonlandırır.

☁️ Bulut Tabanlı Medya Yönetimi

Şarkılar, albüm kapakları ve sanatçı görselleri uygulama sunucusunda değil, Azure Blob Storage üzerinde saklanır. Böylece yüksek ölçeklenebilirlik ve CDN uyumluluğu sağlanır.

🎨 Modern Yönetim Paneli

Hazır admin template'leri kullanılmadan, Vercel ve Linear tasarım anlayışından ilham alınarak tamamen özel geliştirilmiş yönetim paneli sunulmaktadır.

🚀 Üretim Odaklı Mimari

Katmanlı mimari, Repository Pattern, servis soyutlamaları ve API tabanlı iletişim sayesinde proje kolayca ölçeklenebilir ve sürdürülebilir yapıdadır.

🏗️ Sistem Mimarisi

Sonara, kullanıcı arayüzü ile iş mantığını birbirinden tamamen ayıran 2-Tier Proxy & Repository Pattern mimarisine sahiptir. Sonara.WebUI katmanı veritabanına asla doğrudan erişmez; tüm veri alışverişi güvenli SonaraApiClient aracılığıyla Sonara.WebApi üzerinden yürütülür.

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

🗂️ Proje Dizin Yapısı

Sonara.sln
├── 📁 Sonara.CoreLayer         # Domain Entity'leri (Song, Artist, Playlist, MembershipPlan vb.)
├── 📁 Sonara.DataAccessLayer   # DbContext, Entity Configurations, Migrations & Repository Impl.
├── 📁 Sonara.DtoLayer          # Strict Request/Response DTO Kontratları
├── 📁 Sonara.WebApi            # REST API, JWT Middleware, Identity Services & Hangfire Jobs
└── 📁 Sonara.WebUI             # MVC Katmanı, Custom CSS Design System & API Client Proxy

⚡ Teknik Derinlik & Özellikler

🎧 Dinleyici & Kullanıcı Deneyimi

Kesintisiz Ses Akışı: Cihazlar arası kaldığı yerden devam edebilen, HTML5 Audio API tabanlı ses oynatıcısı.

Dinamik Metadata Ayıklama: Yüklenen MP3 dosyalarının süresi, bitrate ve kapak bilgileri TagLibSharp ile sunucu tarafında otomatize olarak okunur.

Ruh Haline Göre Keşif: Odaklan, Enerji, Yağmurlu, Yol Modu gibi parametrik ruh hali filtreleme altyapısı.

IDOR Korunmalı Playlist Yönetimi: Sadece ilgili kullanıcının müdahale edebildiği sahiplik doğrulama (Ownership Guard) altyapısı.

Brute-Force Koruması: 5 hatalı giriş denemesinde hesabı 10 dakika otomatik kilitleyen güvenlik politikası.

💳 Tiered Membership (Çok Katmanlı Üyelik)

4 Seviyeli Erişim Katmanı: Free → Basic → Gold → Elite

Anlık Erişim Kısıtlaması: Yetki kapsamı dışındaki içeriklerde otomatik "Yükselt (Upgrade)" yönlendirme mekanizması.

👑 SaaS Admin Paneli

Gerçek Zamanlı Analitik: Katalog dağılımları ve dinleme istatistiklerini içeren dinamik dashboard.

Tam Kapsamlı CRUD: Şarkı, Sanatçı, Album ve Paket yönetimi.

Sürükle-Brak Medya Yükleyici: Görsel ve ses dosyaları için asenkron bulut yükleme arayüzü.

Rol Bazlı Güvenlik: [Authorize(Roles = "Admin")] özniteliği ile tam yetki ayrıştırması.

📸 Ekran Görüntüleri

| Giriş & Kayıt Sayfası | Keşfet (Dashboard) |
<img width="1897" height="910" alt="Ekran görüntüsü 2026-08-25 125049" src="https://github.com/user-attachments/assets/e79cd1bc-7c55-4b39-a927-6526ac07682e" />
<img width="1901" height="913" alt="Ekran görüntüsü 2026-08-25 125102" src="https://github.com/user-attachments/assets/08bee1dc-e2b3-4261-a397-36f0146fa219" />
<img width="1907" height="911" alt="Ekran görüntüsü 2026-08-25 125117" src="https://github.com/user-attachments/assets/8b7f8fcc-e926-4521-a784-b2b6326a1264" />
<img width="1900" height="912" alt="Ekran görüntüsü 2026-08-25 125136" src="https://github.com/user-attachments/assets/e7c7ab0b-8447-4166-929c-defdb1371a9e" />
<img width="1886" height="911" alt="Ekran görüntüsü 2026-08-25 125149" src="https://github.com/user-attachments/assets/314cfe9d-d900-4e7d-a288-dc199925dfd9" />
<img width="1890" height="913" alt="Ekran görüntüsü 2026-08-25 125200" src="https://github.com/user-attachments/assets/f96481f7-edca-4c52-8d0b-b1cc9eccb8e6" />
<img width="1881" height="343" alt="Ekran görüntüsü 2026-08-25 125401" src="https://github.com/user-attachments/assets/ea69e78d-bbde-4db0-abec-69cbf46f2bf5" />
<img width="1887" height="912" alt="Ekran görüntüsü 2026-08-25 125407" src="https://github.com/user-attachments/assets/58c75f63-8ba9-4650-ae3a-802dae7b2b24" />

| Sanatçı Detay | Playlist Detay |
<img width="1898" height="908" alt="Ekran görüntüsü 2026-08-25 125315" src="https://github.com/user-attachments/assets/03c2a531-4cee-47a6-80ec-cf06364ce097" />
<img width="1911" height="914" alt="Ekran görüntüsü 2026-08-25 125328" src="https://github.com/user-attachments/assets/7eea8aac-a7a7-465e-84cc-9d2bc552e73e" />
<img width="1902" height="906" alt="Ekran görüntüsü 2026-08-25 125308" src="https://github.com/user-attachments/assets/d738eaf8-2a40-4ade-87b4-d6f3505c8a72" />

| Tüm Şarkılar | Paket Detay|
<img width="1892" height="915" alt="Ekran görüntüsü 2026-08-25 125339" src="https://github.com/user-attachments/assets/a089b69a-7807-4e05-9cec-bf305ebeeb8c" />
<img width="1893" height="911" alt="Ekran görüntüsü 2026-08-25 125214" src="https://github.com/user-attachments/assets/f63db565-e2a6-4e3f-8150-21f5abb7be42" />



| Admin — Genel Bakış |
<img width="1905" height="915" alt="Ekran görüntüsü 2026-08-25 124856" src="https://github.com/user-attachments/assets/838a5877-4de2-42f2-a512-38d72f0a21b8" />



| Admin — Şarkı Yönetimi | Admin — Sanatçı Yönetimi | Admin — Mod Yönetimi | Admin — Paket Yönetimi |
<img width="1904" height="908" alt="Ekran görüntüsü 2026-08-25 124902" src="https://github.com/user-attachments/assets/c31d59ff-8e49-4a3d-bc68-ed69b9be4651" />
<img width="1897" height="907" alt="Ekran görüntüsü 2026-08-25 124920" src="https://github.com/user-attachments/assets/67e05433-3fb7-44ca-b859-b9e1065ad018" />
<img width="1910" height="902" alt="Ekran görüntüsü 2026-08-25 124930" src="https://github.com/user-attachments/assets/41232a56-5116-428e-b2a7-9a880c454640" />
<img width="1901" height="905" alt="Ekran görüntüsü 2026-08-25 124944" src="https://github.com/user-attachments/assets/0eabe88b-cb4d-4456-89d6-0f02663b08d8" />
<img width="1914" height="905" alt="Ekran görüntüsü 2026-08-25 124951" src="https://github.com/user-attachments/assets/afb57613-b5e7-4d5b-87ca-98d02a60bf3c" />
<img width="1901" height="907" alt="Ekran görüntüsü 2026-08-25 124958" src="https://github.com/user-attachments/assets/ff7de322-992e-43b8-a7d9-7288d0df2a93" />
<img width="1907" height="916" alt="Ekran görüntüsü 2026-08-25 125015" src="https://github.com/user-attachments/assets/1132417e-aa22-4ce1-b577-7a42130cb52d" />
<img width="1898" height="907" alt="Ekran görüntüsü 2026-08-25 125031" src="https://github.com/user-attachments/assets/9d910114-bb00-41c4-8eb5-3994098d3d7f" />



🛠️ Teknoloji Yığını

Backend & Sunucu Katmanı

Framework: ASP.NET Core 10 Web API

ORM & DB: Entity Framework Core 10, SQL Server

Kimlik Doğrulama: ASP.NET Core Identity, JWT Bearer Tokens (HttpOnly Cookie Transport)

Arka Plan Görevleri: Hangfire

Bulut Depolama: Azure Blob Storage SDK

Medya Metadata: TagLibSharp

Object Mapper: AutoMapper

Frontend & Arayüz Katmanı

Mimari: ASP.NET Core MVC (Razor Views)

Scripting: Pure Vanilla JS (ES6+ Native Modüller)

Styling: Custom CSS Design System (Aero/Linear Dark Minimalist Aesthetic)

🚀 Kurulum ve Yapılandırma

Ön Gereksinimler

.NET 10 SDK

SQL Server 2019+ / LocalDB

Azure Storage Account (veya Azurite Emulator)

Adım Adım Çalıştırma

Repoyu Klonlayın:

git clone https://github.com/dilanderegozu/Sonara.git
cd Sonara

User Secrets Yapılandırması (WebApi projesinde):

cd Sonara.WebApi
dotnet user-secrets set "AzureStorage:ConnectionString" "<YOUR_AZURE_CONNECTION_STRING>"
dotnet user-secrets set "JwtSettings:Secret" "<YOUR_LONG_JWT_SECRET_KEY>"

Veritabanı Migration'larını Uygulayın:

dotnet ef database update --project ../Sonara.DataAccessLayer/Sonara.DataAccessLayer.csproj

Uygulamaları Başlatın:

# Terminal 1 - API Servisi
cd Sonara.WebApi
dotnet run

# Terminal 2 - UI Servisi
cd Sonara.WebUI
dotnet run --launch-profile https

Tarayıcıda Açın:
https://localhost:7113

🗺️ Yol Haritası (Roadmap)

Google / Spotify OAuth 2.0 ile Sosyal Giriş Entegrasyonu

SignalR ile Admin Dashboard üzerinde Anlık Dinleyici Sayısı / Canlı Akış Metrikleri

CQRS & MediatR Deseni ile Admin Katmanının Yeniden Yapılandırılması

Redis Distributed Caching ile Sık Dinlenen Şarkı Metadatalarının Önbelleğe Alınması

<div align="center">
  <p>Designed & Developed with ❤️ by <strong>Dilan Deregözü</strong></p>
</div>
