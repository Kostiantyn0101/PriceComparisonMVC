# PriceComparisonMVC

**ASP.NET MVC storefront that consumes the PriceComparison Web API and lets customers browse, filter, compare and buy products from multiple sellers.**

---

## ✨ Key Features

| Area               | Highlights                                                                                       |
| ------------------ | ------------------------------------------------------------------------------------------------ |
| **Catalog**        | Category tree, search‑as‑you‑type, advanced characteristic filters, auction‑ranked listings.     |
| **Product**        | Gallery with zoom, tech‑spec table, AI‑generated comparison, price history chart, reviews & Q/A. |
| **Comparison**     | Side‑by‑side diff up to 5 products; indicates better/worse specs with colour hints.              |
| **Wishlist**       | Add / remove items (local storage for guests, server‑side for logged‑in users).                  |
| **Authentication** | Sign‑up / login – JWT tokens stored in Secure cookies.                                           |
| **Responsive UI**  | Bootstrap 5, server‑side rendering (Razor), unobtrusive validation.                              |
| **Localization**   | UI texts & product units translated (🇺🇦 Ukrainian default, 🇬🇧 English fallback).                 |
| **Caching**        | Smart client‑ & server‑side caching headers to minimise API calls.                               |
| **Dev UX**         | Hot‑reload (`dotnet watch`), Swagger‑powered API client, Serilog request logging.                |

---

## 🛠️ Tech Stack

* **.NET 8**, C# 12
* **ASP.NET Core MVC** (Razor views)
* **Bootstrap 5**
* **RestSharp** for typed API calls

---

## 📂 Repository Layout

```
├── PriceComparisonMVC/              # Main MVC project
│   ├── Controllers/
│   ├── Views/
│   │   ├── Shared/
│   │   └── Products/
│   ├── wwwroot/                    # Static assets
│   ├── Services/                   # API wrappers
│   └── appsettings.json
├── database/                       # Optional SQL seed for local demo (see API repo)
└── PriceComparisonMVC.sln
```

---

## 🚀 Getting Started

### Prerequisites

* **.NET 8 SDK**
* Running instance of [PriceComparison Web API](https://github.com/Kostiantyn0101/PriceComparisonWebAPI)

### 1. Clone & restore

```bash
git clone https://github.com/Kostiantyn0101/PriceComparisonWebAPI.git
cd PriceComparisonWebAPI/PriceComparisonMVC
dotnet restore
```

### 2. Configure settings

Create **PriceComparisonMVC/appsettings.Secrets.json** (ignored by git):

```jsonc
{
  "Jwt": {
    "Key": "example"
  }
}
```

### 3. Run

```bash
dotnet watch run --project PriceComparisonMVC
# http://localhost:5080
```

> Swagger UI of the backend must be available – the UI uses it to validate endpoints during development.

---

## 📸 Screenshots

*(Coming soon – feel free to open a PR with nice screenshots)*

---

## 🗘️ Roadmap

* [ ] **Dark mode** toggle (Bootstrap 5 CSS vars)
* [ ] **Lazy image loading** for long product lists
* [ ] **Client‑side Blazor comparison widget**
* [ ] **PWA** install prompt & offline support

---

## 🔗 Related repositories

* **Backend API** – [Kostiantyn0101/PriceComparisonWebAPI](https://github.com/Kostiantyn0101/PriceComparisonWebAPI)
* **Admin Panel** – [Kostiantyn0101/PriceComparison-UI-MVC-admin](https://github.com/Kostiantyn0101/PriceComparison-UI-MVC-admin)
* **PriceComparison MVC Front‑end** – [Kostiantyn0101/PriceComparison-UI-MVC](https://github.com/Kostiantyn0101/PriceComparison-UI-MVC)

## 🤝 Contributing

All improvements are welcome! Please fork the repo and open a PR.

---

## 📝 License

MIT – see [LICENSE](LICENSE) for details.
