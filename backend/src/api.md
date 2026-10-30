# Florarium API: контракт для контроллеров

Документ описывает, какие HTTP-методы нужны фронтенду, что они принимают и что возвращают. Он собран по экранам из `frontend/src/screens.ts` и сущностям из ветки `feat/domain-models` (коммит `78bb716`).

Статус: **предложение для обсуждения**. Пути, поля и коды ошибок можно менять, но лучше договориться до того, как писать контроллеры, и потом не расходиться. Когда методы будут реализованы, источником правды станет Swagger: http://localhost:8080/swagger.

## Содержание

1. [Общие соглашения](#1-общие-соглашения)
2. [Сводная таблица](#2-сводная-таблица)
3. [Авторизация и профиль](#3-авторизация-и-профиль)
4. [01 Каталог](#4-01-каталог)
5. [02 Карточка букета](#5-02-карточка-букета)
6. [03 Конструктор](#6-03-конструктор)
7. [04 Корзина](#7-04-корзина)
8. [05 Оформление заказа](#8-05-оформление-заказа)
9. [06 Личный кабинет](#9-06-личный-кабинет)
10. [07 Библиотека составов](#10-07-библиотека-составов)
11. [08 Пустые и граничные состояния](#11-08-пустые-и-граничные-состояния)
12. [Чего не хватает в Domain](#12-чего-не-хватает-в-domain)
13. [Вне экранов: флорист и админ](#13-вне-экранов-флорист-и-админ)
14. [Порядок реализации](#14-порядок-реализации)

---

## 1. Общие соглашения

### 1.1 Базовый URL и формат

- Все методы лежат под префиксом `/api`. Исключение: служебный `/health`.
- Тело запросов и ответов в JSON, `Content-Type: application/json; charset=utf-8`.
- Имена полей в `camelCase`, как ASP.NET Core делает по умолчанию.
- Перечисления (enum) передаются строками в `camelCase`: `"inTransit"`, а не `1` и не `"InTransit"`. Настройка в `Program.cs`:

```csharp
builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
```

### 1.2 Типы данных

| Что | В C# | В JSON | Пример |
|---|---|---|---|
| Идентификатор | `Guid` | строка uuid | `"3f2b8c1e-…"` |
| Деньги | `decimal` (`numeric(10,2)`) | число, рубли, 2 знака | `840.00` |
| Момент времени | `DateTime` (UTC) | ISO 8601 с `Z` | `"2026-10-20T12:30:00Z"` |
| Дата без времени | `DateOnly` | `YYYY-MM-DD` | `"2026-10-22"` |
| Количество стеблей / штук | `int` | целое | `7` |

Все `DateTime` хранятся и отдаются в UTC. В локальное время их переводит фронтенд.

### 1.3 Значения enum в JSON

| Enum | Значения |
|---|---|
| `CompositionSource` | `template`, `custom` |
| `OrderStatus` | `new`, `paid`, `assembling`, `delivering`, `done`, `cancelled` |
| `DeliveryStatus` | `pending`, `inTransit`, `delivered`, `failed` |
| `PaymentMethod` | `card`, `cash`, `online` |
| `PaymentStatus` | `pending`, `paid`, `refunded`, `failed` |

Как понимать `PaymentMethod` (стоит подтвердить): `card` — картой курьеру, `cash` — наличными курьеру, `online` — оплата на сайте сразу после оформления.

### 1.4 Пагинация

Списки, которые могут разрастись (каталог, заказы, отзывы, библиотека), принимают `page` (с 1, по умолчанию 1) и `pageSize` (по умолчанию 12, максимум 50) и возвращают обёртку:

```json
{
  "items": [],
  "total": 0,
  "page": 1,
  "pageSize": 12
}
```

Короткие справочники (цветы, упаковки, ленты, адреса) отдаются обычным массивом без пагинации.

### 1.5 Ошибки

Формат ошибок: стандартный `ProblemDetails` (RFC 9457), включается через `builder.Services.AddProblemDetails()`. Дополнительно у ошибки есть поле `code`: по нему фронтенд выбирает, что показать, и не разбирает текст.

```json
{
  "status": 409,
  "title": "Недостаточно цветов на складе",
  "code": "out_of_stock",
  "detail": "Пион Sarah Bernhardt: запрошено 50, свободно 40",
  "shortages": [
    { "flowerId": "…", "name": "Пион Sarah Bernhardt", "requested": 50, "available": 40 }
  ]
}
```

Ошибки валидации (FluentValidation уже подключён в `Florarium.Api.csproj`) возвращаются как `ValidationProblemDetails` со статусом 400:

```json
{
  "status": 400,
  "title": "One or more validation errors occurred.",
  "code": "validation_failed",
  "errors": {
    "items[0].qty": ["Количество должно быть больше 0"],
    "cardText": ["Не длиннее 500 символов"]
  }
}
```

| HTTP | Когда |
|---|---|
| 200 | Успешное чтение или изменение, в ответе есть тело |
| 201 | Создан ресурс. В заголовке `Location` его URL, в теле сам ресурс |
| 204 | Успешно, тела нет (удаление, выход) |
| 400 | Невалидный запрос (`validation_failed`) |
| 401 | Не авторизован: фронтенд отправляет на вход |
| 403 | Авторизован, но нет прав (например, чужой заказ при включённой роли) |
| 404 | Не найдено, **в том числе чужой ресурс**: не раскрываем, что он существует |
| 409 | Конфликт бизнес-правил (нет остатка, заказ нельзя отменить и т. д.) |

Коды бизнес-ошибок:

| `code` | HTTP | Где возникает |
|---|---|---|
| `validation_failed` | 400 | Везде |
| `invalid_credentials` | 401 | `POST /api/auth/login` |
| `email_taken` | 409 | `POST /api/auth/register` |
| `out_of_stock` | 409 | Добавление в корзину, изменение количества, повтор заказа |
| `flower_inactive` | 409 | Состав содержит снятый с продажи цветок |
| `composition_in_use` | 409 | Правка или удаление состава, который уже в корзине или в заказе |
| `cart_empty` | 409 | `POST /api/orders` |
| `reservation_expired` | 409 | `POST /api/orders`, если резерв истёк и не продлевается |
| `slot_unavailable` | 409 | `POST /api/orders` |
| `order_not_cancellable` | 409 | `POST /api/orders/{id}/cancel` |
| `order_already_paid` | 409 | `POST /api/orders/{id}/payments` |
| `order_not_done` | 409 | Отзыв на ещё не доставленный заказ |
| `already_rated` | 409 | Повторный отзыв на ту же позицию |

### 1.6 Авторизация и роли

- Предлагается **cookie-аутентификация** ASP.NET Core: HttpOnly-кука, никаких токенов в `localStorage`.
- По умолчанию cookie-схема при 401/403 отвечает редиректом 302 на `/Account/Login`, а SPA это ломает. Нужно переопределить события, чтобы API отдавал чистые 401/403:

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.ExpireTimeSpan = TimeSpan.FromDays(14);
        o.SlidingExpiration = true;
        o.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; };
        o.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; };
    });
```

- В разработке фронтенд ходит в API через прокси Vite (`/api` → `http://localhost:8080`), поэтому кука same-origin и CORS с `AllowCredentials` не нужен.
- Роли берутся из сида: `client`, `florist`, `admin`. Все методы из этого документа рассчитаны на `client`, методы флориста и админа вынесены в [раздел 13](#13-вне-экранов-флорист-и-админ).
- Без входа доступны: каталог, карточка букета, справочники, конструктор с расчётом цены. Корзина, заказы, профиль и сохранение состава требуют входа, потому что `Cart.CustomerId` и `Composition.AuthorId` обязательные.

---

## 2. Сводная таблица

Обозначения в колонке «Вход»: 🔓 доступно без входа, 🔒 нужен вход.

| Метод | Путь | Вход | Экран | Контроллер |
|---|---|---|---|---|
| POST | `/api/auth/register` | 🔓 | вход | `AuthController` |
| POST | `/api/auth/login` | 🔓 | вход | `AuthController` |
| POST | `/api/auth/logout` | 🔒 | шапка | `AuthController` |
| GET | `/api/me` | 🔒 | все | `MeController` |
| PATCH | `/api/me` | 🔒 | 06 | `MeController` |
| GET | `/api/bouquets` | 🔓 | 01 | `BouquetsController` |
| GET | `/api/flowers/colors` | 🔓 | 01 | `FlowersController` |
| GET | `/api/bouquets/{id}` | 🔓 | 02 | `BouquetsController` |
| GET | `/api/bouquets/{id}/ratings` | 🔓 | 02 | `BouquetsController` |
| GET | `/api/flowers` | 🔓 | 03 | `FlowersController` |
| GET | `/api/packagings` | 🔓 | 03 | `DictionariesController` |
| GET | `/api/ribbons` | 🔓 | 03 | `DictionariesController` |
| POST | `/api/compositions/quote` | 🔓 | 03 | `CompositionsController` |
| POST | `/api/compositions` | 🔒 | 03 | `CompositionsController` |
| GET | `/api/compositions/{id}` | 🔓/🔒 | 03 | `CompositionsController` |
| PUT | `/api/compositions/{id}` | 🔒 | 03 | `CompositionsController` |
| GET | `/api/cart` | 🔒 | 04, шапка | `CartController` |
| POST | `/api/cart/items` | 🔒 | 02, 03, 04, 07 | `CartController` |
| PATCH | `/api/cart/items/{id}` | 🔒 | 04 | `CartController` |
| DELETE | `/api/cart/items/{id}` | 🔒 | 04 | `CartController` |
| GET | `/api/me/addresses` | 🔒 | 05, 06 | `MeController` |
| POST | `/api/me/addresses` | 🔒 | 05, 06 | `MeController` |
| PUT | `/api/me/addresses/{id}` | 🔒 | 06 | `MeController` |
| DELETE | `/api/me/addresses/{id}` | 🔒 | 06 | `MeController` |
| GET | `/api/delivery/slots` | 🔓 | 05 | `DeliveryController` |
| POST | `/api/orders` | 🔒 | 05 | `OrdersController` |
| POST | `/api/orders/{id}/payments` | 🔒 | 05 | `OrdersController` |
| GET | `/api/me/orders` | 🔒 | 06 | `MeController` |
| GET | `/api/orders/{id}` | 🔒 | 05, 06 | `OrdersController` |
| POST | `/api/orders/{id}/cancel` | 🔒 | 06 | `OrdersController` |
| POST | `/api/orders/{id}/repeat` | 🔒 | 06 | `OrdersController` |
| POST | `/api/order-items/{id}/rating` | 🔒 | 06 | `RatingsController` |
| GET | `/api/me/compositions` | 🔒 | 07 | `MeController` |
| DELETE | `/api/compositions/{id}` | 🔒 | 07 | `CompositionsController` |

---

## 3. Авторизация и профиль

### `POST /api/auth/register`

Регистрирует покупателя с ролью `client` и сразу логинит его (ставит куку).

Тело:

```json
{
  "name": "Иван Петров",
  "email": "ivan@example.com",
  "password": "не короче 8 символов",
  "phone": "+7 900 123-45-67"
}
```

| Поле | Обязательно | Правила |
|---|---|---|
| `name` | да | 1–200 символов (`User.Name`) |
| `email` | да | валидный e-mail, до 200 символов, перед проверкой уникальности приводится к нижнему регистру |
| `password` | да | от 8 символов. Хранится как хэш через `PasswordHasher<User>`, см. [раздел 12](#12-чего-не-хватает-в-domain) |
| `phone` | нет | до 20 символов |

Ответ `201`: объект пользователя, как в `GET /api/me`.
Ошибки: `400 validation_failed`, `409 email_taken`.

### `POST /api/auth/login`

```json
{ "email": "client@florarium.local", "password": "…" }
```

Ответ `200`: объект пользователя, как в `GET /api/me`, и кука.
Ошибки: `401 invalid_credentials`. Сообщение одно и то же для «нет такого e-mail» и «неверный пароль».

### `POST /api/auth/logout`

Удаляет куку. Ответ `204`.

### `GET /api/me`

Текущий пользователь. Фронтенд вызывает его при старте приложения: `200` означает, что пользователь вошёл, `401` — гость.

```json
{
  "id": "…",
  "name": "Иван Петров",
  "email": "client@florarium.local",
  "phone": "+7 900 123-45-67",
  "role": "client",
  "registeredAt": "2026-10-16T10:43:35Z"
}
```

### `PATCH /api/me`

Частичное обновление: можно передать только изменившиеся поля.

```json
{ "name": "Иван Петров", "phone": "+7 900 765-43-21" }
```

E-mail и пароль здесь не меняются. Для них при необходимости позже сделать отдельные методы.
Ответ `200`: обновлённый пользователь.

---

## 4. 01 Каталог

Экран `/`. Это витрина готовых букетов. **Букет** здесь — `Composition` с `Source = Template`.

### `GET /api/bouquets`

Параметры запроса:

| Параметр | Тип | По умолчанию | Описание |
|---|---|---|---|
| `color` | string, можно повторять | — | Значение `Flower.ColorFamily`. Букет подходит, если **хотя бы один** цветок в нём этого цвета. `?color=белый&color=розовый` значит «или» |
| `priceFrom` | number | — | `Composition.Price >= priceFrom` |
| `priceTo` | number | — | `Composition.Price <= priceTo` |
| `inStock` | bool | `false` | `true`: только букеты, которые можно собрать прямо сейчас |
| `sort` | string | `popular` | `popular` (по числу заказанных штук), `priceAsc`, `priceDesc`, `new` (по `CreatedAt`) |
| `page`, `pageSize` | int | 1, 12 | см. [1.4](#14-пагинация) |

Ответ `200`:

```json
{
  "items": [
    {
      "id": "…",
      "title": "Весенний микс",
      "photoUrl": null,
      "price": 840.00,
      "colors": ["жёлтый", "белый"],
      "availability": { "inStock": true, "maxQty": 42 },
      "rating": { "avg": 5.0, "count": 1 }
    }
  ],
  "total": 2,
  "page": 1,
  "pageSize": 12
}
```

Как считаются поля:

- `title`: в Domain такого поля пока нет, сид сейчас пишет название в `CardText`. См. [раздел 12](#12-чего-не-хватает-в-domain).
- `colors`: уникальные `ColorFamily` цветов из `CompositionItem`.
- `availability.maxQty`: сколько таких букетов можно собрать из свободного остатка, то есть `min по составу( floor((Stock.Available − Stock.Reserved) / item.Qty) )`. `inStock = maxQty > 0`. Если в составе есть цветок с `IsActive = false`, то `inStock = false`.
- `rating`: среднее и количество по `Rating` через `OrderItem.CompositionId = букет`.

Пустой результат по фильтрам — это `200` с `items: []` и `total: 0`, а не 404.

### `GET /api/flowers/colors`

Значения для фильтра по цвету: уникальные `ColorFamily` активных цветов, отсортированные по алфавиту.

```json
["белый", "голубой", "жёлтый", "красный", "розовый"]
```

---

## 5. 02 Карточка букета

Экран `/bouquet/:id`.

### `GET /api/bouquets/{id}`

Возвращает только шаблоны (`Source = Template`). Для чужого `custom`-состава ответ `404`.

```json
{
  "id": "…",
  "title": "Красная классика",
  "photoUrl": null,
  "source": "template",
  "price": 3350.00,
  "priceBreakdown": {
    "flowers": 3000.00,
    "packaging": 350.00
  },
  "items": [
    {
      "flower": {
        "id": "…",
        "name": "Роза Red Naomi",
        "variety": null,
        "colorFamily": "красный",
        "photoUrl": null,
        "pricePerStem": 180.00
      },
      "qty": 15,
      "lineTotal": 2700.00
    },
    {
      "flower": { "id": "…", "name": "Гипсофила", "variety": null, "colorFamily": "белый", "photoUrl": null, "pricePerStem": 60.00 },
      "qty": 5,
      "lineTotal": 300.00
    }
  ],
  "packaging": { "id": "…", "title": "Подарочная коробка", "price": 350.00 },
  "ribbon": { "id": "…", "title": "Атласная белая", "colorHex": "#FFFFFF" },
  "availability": { "inStock": true, "maxQty": 13, "backInStockAt": null },
  "rating": { "avg": null, "count": 0 }
}
```

- Цена букета: `Σ(qty × Flower.PricePerStem) + Packaging.Price`. Лента бесплатная, у `Ribbon` нет цены. Хранится в `Composition.Price`.
- `availability.backInStockAt`: самая поздняя `Stock.BackInStockAt` среди цветов, которых не хватает. Если всего хватает, то `null`.
- Кнопка «В корзину» вызывает `POST /api/cart/items`. Кнопка «Изменить состав» ведёт на `/constructor/{id}` (см. [раздел 6](#6-03-конструктор)).

### `GET /api/bouquets/{id}/ratings`

Отзывы на букет. Связь такая: `Rating → OrderItem → Composition`.

Параметры: `page`, `pageSize`.

```json
{
  "summary": {
    "avg": 4.5,
    "count": 2,
    "distribution": { "1": 0, "2": 0, "3": 0, "4": 1, "5": 1 }
  },
  "items": [
    {
      "id": "…",
      "stars": 5,
      "tags": ["свежие", "как на фото"],
      "text": "Простояли неделю",
      "authorName": "Иван",
      "createdAt": "2026-10-16T10:43:35Z"
    }
  ],
  "total": 2,
  "page": 1,
  "pageSize": 12
}
```

`authorName`: только имя (первое слово `User.Name`), без фамилии и e-mail.

---

## 6. 03 Конструктор

Экран `/constructor/:id?`.

- Без `id` — пустой конструктор.
- С `id` шаблона: состав букета загружается как стартовый. Сохранение **всегда создаёт новый** `custom`-состав, шаблон не меняется.
- С `id` своего `custom`-состава: редактирование. Чужой состав открыть нельзя.

### `GET /api/flowers`

Цветы для палитры конструктора. Без пагинации, цветов немного.

| Параметр | Тип | Описание |
|---|---|---|
| `color` | string, можно повторять | фильтр по `ColorFamily` |
| `search` | string | поиск по `Name` и `Variety` без учёта регистра (`ILIKE`) |
| `onlyAvailable` | bool | только с `free > 0` |

Возвращаются только `IsActive = true`.

```json
[
  {
    "id": "…",
    "name": "Пион Sarah Bernhardt",
    "variety": null,
    "latinName": null,
    "colorFamily": "розовый",
    "photoUrl": null,
    "pricePerStem": 450.00,
    "free": 40,
    "backInStockAt": null
  }
]
```

`free = Stock.Available − Stock.Reserved`. Отдельно `available` и `reserved` клиенту не нужны.

### `GET /api/packagings`

```json
[
  { "id": "…", "title": "Крафт-бумага", "price": 100.00 },
  { "id": "…", "title": "Подарочная коробка", "price": 350.00 }
]
```

### `GET /api/ribbons`

```json
[
  { "id": "…", "title": "Атласная роза", "colorHex": "#FF6B9D" },
  { "id": "…", "title": "Атласная белая", "colorHex": "#FFFFFF" }
]
```

### `POST /api/compositions/quote`

Пересчёт цены **без сохранения**. Фронтенд вызывает его при каждом изменении состава, с debounce примерно 300 мс. Цену считает сервер, чтобы она не расходилась с той, по которой потом посчитают корзину.

Тело (общее с `POST`/`PUT /api/compositions`):

```json
{
  "items": [
    { "flowerId": "…", "qty": 7 },
    { "flowerId": "…", "qty": 3 }
  ],
  "packagingId": "…",
  "ribbonId": "…"
}
```

Ответ `200`:

```json
{
  "items": [
    { "flowerId": "…", "qty": 7, "pricePerStem": 80.00, "lineTotal": 560.00 },
    { "flowerId": "…", "qty": 3, "pricePerStem": 60.00, "lineTotal": 180.00 }
  ],
  "flowersTotal": 740.00,
  "packagingPrice": 100.00,
  "total": 840.00,
  "warnings": [
    { "code": "out_of_stock", "flowerId": "…", "requested": 50, "available": 40 }
  ]
}
```

Нехватка остатка здесь не ошибка, а `warnings`: собирать букет можно, а положить в корзину не получится.

### `POST /api/compositions`

Сохраняет свой состав. Сервер ставит `AuthorId = текущий пользователь`, `Source = custom` и сам считает `Price`.

```json
{
  "items": [
    { "flowerId": "…", "qty": 7 },
    { "flowerId": "…", "qty": 3 }
  ],
  "packagingId": "…",
  "ribbonId": "…",
  "cardText": "С днём рождения!"
}
```

Валидация:

| Правило | Код |
|---|---|
| `items` не пустой, не больше 30 позиций | `validation_failed` |
| `qty` от 1 до 200 | `validation_failed` |
| `flowerId` в `items` не повторяются (уникальный индекс `composition_id + flower_id`) | `validation_failed` |
| Все цветы существуют и `IsActive = true` | `flower_inactive` |
| `packagingId` и `ribbonId`, если переданы, существуют | `validation_failed` |
| `cardText` до 500 символов | `validation_failed` |

Остаток при сохранении **не проверяется**: состав можно сохранить в библиотеку, даже если сейчас цветов нет.

Ответ `201`, `Location: /api/compositions/{id}`. Тело в формате `GET /api/compositions/{id}`.

### `GET /api/compositions/{id}`

Открывает состав в конструкторе. Шаблон доступен всем, `custom` — только автору (иначе `404`).

Формат тот же, что у `GET /api/bouquets/{id}`, и ещё два поля:

```json
{
  "cardText": "С днём рождения!",
  "isEditable": true
}
```

`isEditable = true`, если это свой `custom`-состав, который нигде не используется (см. `PUT`). Если `false`, фронтенд показывает только «Сохранить как новый».

### `PUT /api/compositions/{id}`

Полная замена состава. Тело такое же, как у `POST`.

- Только автор и только `Source = custom`, иначе `404`.
- Если состав уже лежит в корзине (`CartItem`) или есть в заказе (`OrderItem`), то `409 composition_in_use`. Иначе пришлось бы менять уже оформленный заказ и пересчитывать резервы. Фронтенд в этом случае предлагает «Сохранить как новый», то есть `POST`.
- Сервер заново считает `Price`.

Ответ `200`: обновлённый состав.

---

## 7. 04 Корзина

Экран `/cart` и счётчик в шапке (`SiteHeader`, `cartCount`). Корзина одна на пользователя и создаётся при первом добавлении.

### Механика резерва

Это главная логика корзины, её нужно сделать аккуратно.

1. При добавлении позиции на каждый цветок состава создаётся `StockReservation` с `Qty = CompositionItem.Qty × CartItem.Qty` и `ExpiresAt = сейчас + TTL`. На ту же величину увеличивается `Stock.Reserved`.
2. Свободный остаток: `Stock.Available − Stock.Reserved`. Если его не хватает хотя бы по одному цветку, позиция не добавляется: `409 out_of_stock` с перечнем `shortages`.
3. TTL резерва предлагается 15 минут (настройка `Reservations:TtlMinutes` в `appsettings`). Любое изменение корзины продлевает все её резервы.
4. Истёкшие резервы снимает фоновая задача (`BackgroundService` раз в минуту): `Stock.Reserved -= Qty`, строка резерва удаляется.
5. При `GET /api/cart` сервер пытается заново зарезервировать позиции с истёкшим резервом. Если остатка не хватает, у позиции `status = "outOfStock"`.
6. При оформлении заказа: `Stock.Available -= Qty`, `Stock.Reserved -= Qty`, резервы удаляются.
7. Все изменения остатков делать в одной транзакции с оптимистичной блокировкой строк `stock` (`uint Version` + `.IsRowVersion()`, Npgsql маппит это на `xmin`). Иначе два покупателя одновременно заберут последние стебли.

### `GET /api/cart`

```json
{
  "items": [
    {
      "id": "…",
      "composition": {
        "id": "…",
        "title": "Весенний микс",
        "photoUrl": null,
        "source": "template"
      },
      "qty": 2,
      "unitPrice": 840.00,
      "lineTotal": 1680.00,
      "status": "reserved",
      "maxQty": 42
    }
  ],
  "count": 1,
  "subtotal": 1680.00,
  "discount": 0.00,
  "total": 1680.00,
  "reservedUntil": "2026-10-20T12:45:00Z"
}
```

| Поле | Смысл |
|---|---|
| `items[].unitPrice` | Цена, зафиксированная при добавлении (`CartItem.UnitPrice`) |
| `items[].status` | `reserved` — стебли отложены; `outOfStock` — резерв истёк и восстановить его не удалось |
| `items[].maxQty` | Максимум для поля «количество»: текущий `qty` плюс то, что ещё можно собрать |
| `count` | Число позиций (строк). Это значение для счётчика в шапке |
| `reservedUntil` | Самый ранний `ExpiresAt` среди резервов, для таймера «товар отложен до…». Если корзина пустая, `null` |

Для гостя ответ `401`, в шапке тогда просто 0.
Пустая корзина: `200`, `items: []`, `count: 0`, суммы по нулям.

### `POST /api/cart/items`

Добавить букет или свой состав.

```json
{ "compositionId": "…", "qty": 1 }
```

- Если этот `compositionId` уже есть в корзине, увеличивается `qty` существующей позиции, новая строка не создаётся.
- Добавлять можно шаблон или свой `custom`. Чужой `custom` даёт `404`.
- `qty` от 1 до 50.

Ответ `200` с **целой корзиной** (в формате `GET /api/cart`), чтобы фронтенд одним запросом обновил и страницу, и счётчик.
Ошибки: `404`, `409 out_of_stock`, `409 flower_inactive`.

### `PATCH /api/cart/items/{id}`

```json
{ "qty": 3 }
```

Меняет количество и пересчитывает резервы на разницу. Ответ `200` с целой корзиной.
Ошибки: `404`, `409 out_of_stock`.

### `DELETE /api/cart/items/{id}`

Удаляет позицию и снимает её резервы. Ответ `200` с целой корзиной.

---

## 8. 05 Оформление заказа

Экран `/checkout`.

### `GET /api/me/addresses`

```json
[
  {
    "id": "…",
    "label": "Дом",
    "street": "ул. Ленина, 10",
    "apartment": "15",
    "entrance": "2",
    "floor": "4",
    "intercom": "15К"
  }
]
```

### `POST /api/me/addresses`

Тело такое же, без `id`. Обязательно только `street` (до 300 символов). `label` до 50, остальные поля до 20.
Ответ `201` с адресом.

### `PUT /api/me/addresses/{id}`, `DELETE /api/me/addresses/{id}`

Для управления адресами в личном кабинете. Чужой адрес даёт `404`. `DELETE` отвечает `204`.

### `GET /api/delivery/slots?date=2026-10-22`

Свободные интервалы доставки на дату.

```json
{
  "date": "2026-10-22",
  "slots": [
    { "slot": "10:00–13:00", "available": true },
    { "slot": "13:00–16:00", "available": true },
    { "slot": "16:00–19:00", "available": false },
    { "slot": "19:00–22:00", "available": true }
  ]
}
```

- Отдельной сущности для слотов в Domain нет. Предлагается держать список интервалов и вместимость (сколько доставок на один слот) в `appsettings` (`Delivery:Slots`, `Delivery:SlotCapacity`). Занятость считается так: `count(Delivery where Date = date and Slot = slot and Order.Status != cancelled)`.
- Дата раньше сегодняшней даёт `400`. Для сегодняшней даты недоступны слоты, которые начинаются меньше чем через 2 часа.

### `POST /api/orders`

Переносит корзину в заказ.

```json
{
  "addressId": "…",
  "recipientName": "Анна Петрова",
  "recipientPhone": "+7 900 000-00-00",
  "date": "2026-10-22",
  "slot": "10:00–13:00",
  "courierNote": "Позвонить за 30 минут",
  "paymentMethod": "online"
}
```

| Поле | Обязательно | Правила |
|---|---|---|
| `addressId` | да | адрес текущего пользователя |
| `recipientName` | нет | до 200 символов. Если не передано, берётся `User.Name` |
| `recipientPhone` | нет | до 20 символов. **Поля нет в Domain**, см. [раздел 12](#12-чего-не-хватает-в-domain) |
| `date` | да | не раньше сегодня |
| `slot` | да | один из слотов из `GET /api/delivery/slots` |
| `courierNote` | нет | до 500 символов |
| `paymentMethod` | да | `card` / `cash` / `online` |

Что делает сервер, всё в одной транзакции:

1. Проверяет, что корзина не пустая (`409 cart_empty`), все резервы живы или продлеваются (`409 reservation_expired` или `out_of_stock`), слот свободен (`409 slot_unavailable`).
2. Создаёт `Order`: `Number` по шаблону `ORD-{год}-{4 цифры}` из последовательности PostgreSQL, `Status = new`, `Subtotal = Σ(qty × unitPrice)`, `Discount = 0`, `Total = Subtotal − Discount`.
3. Создаёт `OrderItem` из каждой `CartItem`, переносит `UnitPrice` как есть.
4. Создаёт `Delivery` (`Status = pending`) и `Payment` (`Method = paymentMethod`, `Status = pending`, `Amount = Total`).
5. Списывает остатки и удаляет резервы (см. [механику резерва](#механика-резерва)).
6. Очищает корзину.

Повторное нажатие «Оформить» вернёт `409 cart_empty`, потому что корзина уже пустая. Это заодно защищает от двойных заказов.

Ответ `201`, `Location: /api/orders/{id}`:

```json
{
  "id": "…",
  "number": "ORD-2026-0002",
  "status": "new",
  "total": 1680.00,
  "paymentMethod": "online",
  "paymentRequired": true
}
```

`paymentRequired = true` только для `online`. Тогда фронтенд сразу вызывает оплату.

### `POST /api/orders/{id}/payments`

Оплата заказа с методом `online`. В учебном проекте без настоящего эквайринга это заглушка, которая всегда проходит успешно.

Тело пустое (`{}`).

Что делает сервер:

- Находит `Payment` заказа со `Status = pending` и ставит ему `Status = paid` и `PaidAt = сейчас`.
- Переводит `Order.Status` в `paid`.

Ответ `200`: заказ в формате `GET /api/orders/{id}`.
Ошибки: `404`, `409 order_already_paid`, `400`, если метод заказа не `online`.

Если позже подключат настоящую оплату, метод начнёт возвращать `{ "redirectUrl": "…" }`, а статус будет меняться по вебхуку от платёжки.

---

## 9. 06 Личный кабинет

Экран `/profile`: данные пользователя (`GET/PATCH /api/me`), адреса (раздел 8), история заказов, отзывы.

### Статусы заказа

```
new ──(оплата онлайн)──► paid ──► assembling ──► delivering ──► done
 │                        │           ▲
 ├──(card / cash)─────────┼───────────┘
 │                        │
 └───────► cancelled ◄────┘   (клиент может отменить только из new и paid)
```

- `new → paid`: прошла онлайн-оплата.
- `new → assembling` (при оплате курьеру) и `paid → assembling`: флорист начал сборку.
- `assembling → delivering → done`: двигает флорист или курьер (методы из [раздела 13](#13-вне-экранов-флорист-и-админ)).
- `new | paid → cancelled`: отмена клиентом. Если оплата была, `Payment.Status = refunded`.

`DeliveryStatus` идёт параллельно: `pending → inTransit → delivered | failed`. При `delivered` заказ переходит в `done`.

### `GET /api/me/orders`

| Параметр | Значения |
|---|---|
| `status` | `active` (new, paid, assembling, delivering), `done`, `cancelled`. Без параметра возвращаются все |
| `page`, `pageSize` | см. [1.4](#14-пагинация) |

Сортировка: новые сверху.

```json
{
  "items": [
    {
      "id": "…",
      "number": "ORD-2026-0001",
      "status": "done",
      "createdAt": "2026-10-14T10:43:35Z",
      "total": 840.00,
      "itemsCount": 1,
      "previewPhotoUrl": null,
      "delivery": { "date": "2026-10-15", "slot": null, "status": "delivered" }
    }
  ],
  "total": 1,
  "page": 1,
  "pageSize": 12
}
```

### `GET /api/orders/{id}`

Полная информация о заказе. Чужой заказ даёт `404`.

```json
{
  "id": "…",
  "number": "ORD-2026-0001",
  "status": "done",
  "createdAt": "2026-10-14T10:43:35Z",
  "subtotal": 840.00,
  "discount": 0.00,
  "total": 840.00,
  "items": [
    {
      "id": "…",
      "composition": { "id": "…", "title": "Весенний микс", "photoUrl": null, "source": "template" },
      "qty": 1,
      "unitPrice": 840.00,
      "lineTotal": 840.00,
      "rating": null,
      "canRate": true
    }
  ],
  "delivery": {
    "recipientName": "Иван Петров",
    "date": "2026-10-15",
    "slot": null,
    "courierNote": null,
    "status": "delivered",
    "deliveredAt": "2026-10-15T09:00:00Z"
  },
  "payments": [
    { "id": "…", "method": "card", "status": "paid", "amount": 840.00, "paidAt": "2026-10-14T11:00:00Z" }
  ],
  "canCancel": false,
  "canRepeat": true
}
```

Флаги `canCancel`, `canRepeat` и `items[].canRate` считает сервер, чтобы правила не дублировались во фронтенде:

| Флаг | Условие |
|---|---|
| `canCancel` | `status ∈ {new, paid}` |
| `canRepeat` | `status ∈ {done, cancelled}` |
| `canRate` | `status = done` и у позиции ещё нет `Rating` |

Если отзыв уже оставлен, в `items[].rating` лежит `{ "stars": 5, "tags": [...], "text": "...", "createdAt": "..." }`.

### `POST /api/orders/{id}/cancel`

Тело пустое. Если `status ∉ {new, paid}`, ответ `409 order_not_cancellable`.

Что делает сервер:

- `Order.Status = cancelled`.
- Возвращает стебли на склад: `Stock.Available += Qty` по всем позициям, потому что букет ещё не начали собирать.
- Если был `Payment` в статусе `paid`, ставит ему `refunded`.

Ответ `200`: заказ в формате `GET /api/orders/{id}`.

### `POST /api/orders/{id}/repeat`

Кладёт позиции заказа в корзину по **текущим** ценам, не по старым `UnitPrice`, и резервирует стебли. То, что добавить не удалось, не роняет запрос, а возвращается списком `skipped`.

```json
{
  "cart": { "...": "в формате GET /api/cart" },
  "skipped": [
    { "compositionId": "…", "title": "Красная классика", "reason": "out_of_stock" }
  ]
}
```

`reason`: `out_of_stock` или `flower_inactive`.
Ответ `200`.

### `POST /api/order-items/{id}/rating`

Отзыв на позицию заказа (один на позицию, есть уникальный индекс `order_item_id`).

```json
{
  "stars": 5,
  "tags": ["свежие", "как на фото"],
  "text": "Простояли неделю"
}
```

| Поле | Правила |
|---|---|
| `stars` | целое от 1 до 5 (`smallint`) |
| `tags` | до 5 строк, каждая до 30 символов (`text[]`). Список подсказок держит фронтенд |
| `text` | до 2000 символов, необязательно |

Ответ `201` с отзывом.
Ошибки: `404` (не своя позиция), `409 order_not_done`, `409 already_rated`.

---

## 10. 07 Библиотека составов

Экран `/library`. В исходном списке методов его не было. Предполагаем, что это сохранённые составы пользователя (`custom`), из которых можно снова заказать или открыть в конструкторе. Если задумано иначе (например, публичная библиотека шаблонов флориста), раздел нужно переписать.

### `GET /api/me/compositions`

Свои `custom`-составы, новые сверху. Параметры: `page`, `pageSize`.

```json
{
  "items": [
    {
      "id": "…",
      "title": null,
      "photoUrl": null,
      "price": 1460.00,
      "flowersPreview": ["Пион Sarah Bernhardt × 3", "Гипсофила × 2"],
      "createdAt": "2026-10-19T08:00:00Z",
      "availability": { "inStock": true, "maxQty": 13 },
      "ordersCount": 0,
      "isEditable": true
    }
  ],
  "total": 1,
  "page": 1,
  "pageSize": 12
}
```

Действия на карточке:

- «В корзину»: `POST /api/cart/items`.
- «Открыть в конструкторе»: переход на `/constructor/{id}`, дальше `GET /api/compositions/{id}`.
- «Удалить»: `DELETE /api/compositions/{id}`.

### `DELETE /api/compositions/{id}`

Удаляет свой `custom`-состав. Ответ `204`.

- Если состав есть в заказе или в корзине, ответ `409 composition_in_use`. В БД стоит `Restrict` на `order_item.composition_id` и `cart_item.composition_id`, так что без проверки упадёт исключение базы.
- Шаблон или чужой состав дают `404`.

---

## 11. 08 Пустые и граничные состояния

Экран `/states` — это витрина состояний, отдельных методов ему не нужно. Ниже показано, по какому ответу API фронтенд узнаёт каждое состояние.

| Состояние | Где | Как узнать |
|---|---|---|
| Ничего не найдено по фильтрам | 01 | `GET /api/bouquets` → `total: 0` |
| Букет закончился | 01, 02 | `availability.inStock = false`, дата в `backInStockAt` |
| Цветок закончился | 03 | `free = 0`, дата в `backInStockAt` |
| Не хватает стеблей при сборке | 03 | `quote` → `warnings[].code = out_of_stock` |
| Не хватает стеблей при добавлении в корзину | 02, 03, 04 | `409 out_of_stock` + `shortages` |
| Цветок сняли с продажи | 03, 04 | `409 flower_inactive` |
| Пустая корзина | 04 | `GET /api/cart` → `count: 0` |
| Резерв истёк, остатка нет | 04 | `items[].status = outOfStock` |
| Таймер резерва | 04 | `reservedUntil` |
| Нет свободных слотов на дату | 05 | все `slots[].available = false` |
| Корзина опустела во время оформления | 05 | `409 cart_empty` |
| Нет адресов | 05, 06 | `GET /api/me/addresses` → `[]` |
| Нет заказов | 06 | `GET /api/me/orders` → `total: 0` |
| Пустая библиотека | 07 | `GET /api/me/compositions` → `total: 0` |
| Не вошёл | все 🔒 | `401` |
| Не найдено | 02, 03, 06 | `404` |
| Сервер или БД недоступны | все | сетевая ошибка или `/health` → `503` |

---

## 12. Чего не хватает в Domain

Методы выше опираются на несколько полей и правил, которых в `feat/domain-models` пока нет. Это нужно решить до контроллеров: каждое изменение здесь — новая миграция.

| # | Что | Зачем | Предложение |
|---|---|---|---|
| 1 | `User.PasswordHash` | Без него нет регистрации и входа | `string PasswordHash`, `HasMaxLength(200).IsRequired()`. Хэш через `PasswordHasher<User>` из `Microsoft.AspNetCore.Identity` (отдельный пакет не нужен, класс есть в shared framework ASP.NET Core) |
| 2 | `Composition.Title` | У букета в каталоге, корзине, заказах и библиотеке должно быть название. Сейчас сид пишет его в `CardText`, а это текст открытки | `string? Title`, `HasMaxLength(200)`. Для шаблонов обязателен (проверка в валидаторе), для `custom` по желанию. В сиде перенести «Весенний микс» и «Красная классика» из `CardText` в `Title` |
| 3 | Адрес в `Delivery` | Сейчас доставка не знает, куда ехать | Хранить **снимок** адреса прямо в `Delivery` (`Street`, `Apartment`, `Entrance`, `Floor`, `Intercom`). Ссылка на `Address` не подходит: если клиент потом изменит или удалит адрес, история заказа поменяется |
| 4 | `Delivery.RecipientPhone` | Курьеру нужен телефон получателя, если это не сам заказчик | `string?`, `HasMaxLength(20)` |
| 5 | Уникальность `cart.customer_id` | Корзина одна на пользователя, а индекс сейчас не уникальный | `b.HasIndex(x => x.CustomerId).IsUnique()` и `User.Cart` вместо `.WithMany()` |
| 6 | Номер заказа | `Order.Number` уникальный, но откуда его брать, не определено | Последовательность PostgreSQL `order_number_seq` (`modelBuilder.HasSequence<int>("order_number_seq")`), номер собирается как `ORD-{год}-{значение:D4}` |
| 7 | Конкурентный доступ к `stock` | Без блокировки два заказа заберут одни и те же стебли | `uint Version` + `.IsRowVersion()` в `StockConfiguration` |
| 8 | Пересчёт `Composition.Price` | Цена состава хранится, поэтому при изменении `Flower.PricePerStem` или `Packaging.Price` цены шаблонов устаревают | При изменении цены (методы админа) пересчитывать цены всех шаблонов, где есть этот цветок или упаковка. `custom`-составы, уже лежащие в заказах, не трогать |
| 9 | Слоты доставки | Нет сущности | Пока хватит конфигурации в `appsettings` (см. [раздел 8](#8-05-оформление-заказа)) |

---

## 13. Вне экранов: флорист и админ

Во фронтенде пока нет этих экранов. Список нужен, чтобы понимать, кто и как двигает статусы заказов и пополняет склад. Все методы под префиксом `/api/admin`, доступ по ролям.

| Метод | Путь | Роль | Назначение |
|---|---|---|---|
| GET/POST/PUT | `/api/admin/flowers[/{id}]` | admin | Справочник цветов. Удаления нет: снятие с продажи через `IsActive = false` |
| GET/POST | `/api/admin/flowers/{id}/supply-batches` | admin, florist | Приёмка поставки: создаёт `SupplyBatch` и увеличивает `Stock.Available` |
| PATCH | `/api/admin/flowers/{id}/stock` | admin | Ручная корректировка остатка и `BackInStockAt` |
| GET/POST/PUT/DELETE | `/api/admin/packagings[/{id}]`, `/api/admin/ribbons[/{id}]` | admin | Справочники упаковки и лент |
| GET/POST/PUT | `/api/admin/templates[/{id}]` | admin, florist | Шаблоны букетов для каталога (`Source = template`) |
| GET | `/api/admin/orders?status=&date=` | admin, florist | Доска заказов на сборку и доставку |
| POST | `/api/admin/orders/{id}/status` | admin, florist | Перевод по статусам: `assembling`, `delivering`, `done` |
| POST | `/api/admin/deliveries/{id}/status` | admin, florist | `inTransit`, `delivered`, `failed` |

---

## 14. Порядок реализации

Предлагаемая последовательность: каждый шаг можно проверить в Swagger и сразу подключить на фронтенде.

1. **Инфраструктура**: `AddProblemDetails`, `JsonStringEnumConverter`, FluentValidation, общий формат пагинации.
2. **Справочники и каталог, без авторизации**: `GET /api/flowers`, `/api/flowers/colors`, `/api/packagings`, `/api/ribbons`, `/api/bouquets`, `/api/bouquets/{id}`. Сид уже есть, так что фронтенд может сразу верстать экраны 01–03 на живых данных. Перед этим шагом нужен пункт 2 из [раздела 12](#12-чего-не-хватает-в-domain) (`Title`).
3. **Авторизация**: пункт 1 из раздела 12, cookie-схема, `register`, `login`, `logout`, `GET/PATCH /api/me`.
4. **Конструктор**: `quote`, `POST/GET/PUT /api/compositions`.
5. **Корзина с резервами**: самый сложный по логике шаг, плюс фоновая задача снятия резервов. Пункты 5 и 7 из раздела 12.
6. **Заказы**: адреса, слоты, `POST /api/orders`, оплата-заглушка. Пункты 3, 4 и 6 из раздела 12.
7. **Личный кабинет**: список и детали заказов, отмена, повтор, отзывы, `GET /api/bouquets/{id}/ratings`.
8. **Библиотека**: `GET /api/me/compositions`, `DELETE /api/compositions/{id}`.
9. **Методы флориста и админа** ([раздел 13](#13-вне-экранов-флорист-и-админ)).
