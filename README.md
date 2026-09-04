# Url Trimmer

Url Trimmer — це простий сервіс для скорочення посилань із авторизацією, ролями та адмінською панеллю. У проєкті є backend на ASP.NET Core, база на SQLite і frontend на Angular.

## Що вміє

- створювати короткі посилання;
- переходити з короткого коду на оригінальний URL;
- переглядати список посилань;
- видаляти посилання;
- реєструватися, входити в акаунт і виходити;
- керувати користувачами;
- редагувати сторінку About для адміна.

## Стек

- ASP.NET Core 10
- Entity Framework Core
- SQLite
- JWT Authentication
- Angular 22

## Запуск backend

За замовчуванням API працює з локальною SQLite-базою `urlshortener.db`.
Якщо файл бази вже містить дані, застосунок використає їх як є.
За бажанням цю базу можна просто видалити — при наступному запуску вона автоматично створиться заново.

```bash
dotnet run --project Backend/src/UrlShortener.Api
```

Після старту доступні адреси:

- `http://localhost:5068`
- `https://localhost:7189`

## Налаштування

У `Backend/src/UrlShortener.Api/appsettings.json` зберігаються:

- рядок підключення до бази;
- дані дефолтного адміністратора.

## Основні ендпоінти

- `POST /api/auth/login`
- `POST /api/auth/registration`
- `GET /api/auth/users`
- `GET /api/auth/me`
- `POST /api/auth/logout`
- `POST /api/shorturls`
- `GET /api/shorturls`
- `GET /{shortCode}`

## Postman collection

У репозиторії є експортована колекція Postman: `Backend/src/Url-Shoretener.postman_collection.json`.

У ній зібрані запити для:

- авторизації та реєстрації;
- отримання списку користувачів;
- видалення користувача;
- створення, перегляду та видалення коротких посилань;
- редіректу за коротким кодом.

## Frontend

Frontend лежить у папці `Frontend`.

```bash
cd Frontend
ng serve
```

Далі відкрий `http://localhost:4200`.
