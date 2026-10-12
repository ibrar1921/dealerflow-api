# DealerFlow API: local development notes

## Create a dealer
`POST /api/dealers`

```json
{"name": "Northside Property Partners", "region": "London"}
```

Returns HTTP **201** with the created dealer ID, or **400** for invalid name/region.

## Create a listing
`POST /api/listings`

```json
{"dealerId": "INSERT_REAL_DEALER_UUID", "title": "Two bedroom apartment", "price": 285000}
```

Replace the example ID with a value returned from the dealers endpoint. Returns **201** or **400**; a nonexistent dealer gives **404**.

## Query listings
- `GET /api/listings`
- `GET /api/listings?status=published`
- `GET /api/listings?dealerId=INSERT_REAL_DEALER_UUID`
- `GET /api/listings/{id}`

## Change the listing's state
`PATCH /api/listings/{id}/status` with `{"status":"published"}`.

Allowed transitions are `draft → published`, `draft → closed` and `published → closed`. Attempting to reopen closed listings is rejected.

## Current limitations
The API has no authentication, tenancy controls, pagination, transaction orchestration or rate limiting. SQLite is stored in the local working directory (or container filesystem). `EnsureCreated` is suitable only for this prototype. No endpoints delete records.

## API tests
`dotnet test tests/DealerFlow.Tests/DealerFlow.Tests.csproj` validates basic workflows using a temporary SQLite database per test factory.