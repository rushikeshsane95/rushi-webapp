# Morning Design Pattern Series — Adapter Pattern (Full Template + Full Discussion)

**Pattern:** Adapter Pattern  
**Category:** Structural

> **The Core Definition:**  
> The Adapter Pattern converts the interface of an existing class into an interface the client expects. It allows incompatible types to work together by wrapping (“adapting”) a legacy/third‑party component behind a stable internal contract.

---

## The Meteorological Example

You are migrating a large forecasting monolith into a **modular monolith** with multiple forecast domains:
- **Waves**
- **Currents**
- **Atmospheric**

Each module:
- Talks to different data sources (legacy DB, third‑party APIs, internal services)
- Produces different DTO shapes, units, naming, and time semantics

But your platform needs:
- **One uniform response contract** exposed to clients (REST/gRPC/UI)
- Consistent parameter names and units
- Stable client behavior as modules evolve independently

Adapter is the first “boundary tool” that helps you standardize access and contain legacy/foreign interfaces.

---

## Architecture Diagram (Mermaid) — Step-by-Step Evolution

### Step 1 — The Anti-Pattern: Branching “God Service”

Client code (or application service) directly knows about every provider and does mapping inline.

```mermaid
flowchart LR
Client[Client / UI / External App] --> AP[Client Access Point\nController/gRPC Endpoint]
AP --> S[MonolithicForecastService]

S -->|if provider is Legacy| Legacy[LegacyGridModel]
S -->|if provider is ThirdParty| TP[ThirdPartyWeatherClient]

S -->|inline mapping + conversions| Canon[Uniform Forecast Response]
```

**Pain points**
- Adding a provider requires modifying the service (violates **OCP**)
- Mapping logic is duplicated and scattered
- Hard to test (you must set up multiple concrete providers)
- High maintenance tax and fragile changes

---

### Step 2 — Introduce a Stable Contract: `IForecastProvider`

Adapters wrap each incompatible provider and expose a uniform interface.

```mermaid
classDiagram
direction LR

class Client {
  <<External Consumer>>
}
class ForecastController {
  <<Client Access Point>>
  +GetForecast(lat, lon, time)
}
class WeatherService {
  +GetPointForecast(...)
}

class IForecastProvider {
  <<interface>>
  +GetForecastAsync(...)
}

class LegacyGridAdapter
class ThirdPartyAdapter
class LegacyGridModel
class ThirdPartyWeatherClient

Client --> ForecastController
ForecastController --> WeatherService
WeatherService --> IForecastProvider

LegacyGridAdapter ..|> IForecastProvider
ThirdPartyAdapter ..|> IForecastProvider

LegacyGridAdapter --> LegacyGridModel : adapts
ThirdPartyAdapter --> ThirdPartyWeatherClient : adapts
```

**What improved**
- Client access point and app services depend on a stable abstraction
- New providers are added by adding new adapters rather than editing core logic
- Cleaner unit tests (mock `IForecastProvider`)

---

### Step 3 — Add DI Boundary: Wiring Matters

Adapters and their dependencies are composed in the DI container (composition root).

```mermaid
flowchart LR
Client[Client] --> AP[Controller/gRPC Endpoint]
AP --> App[WeatherService]
App -->|depends on| IFP[IForecastProvider]

subgraph DI["DI Container (Composition Root)"]
  IFP --> A[ThirdPartyAdapter]
  A --> C[ThirdPartyWeatherClient]
end

C --> External[3rd Party Weather API]
```

**Key insight**
- The adapter should **not** decide which concrete dependencies to create.
- DI wiring is where you decide which implementation is active.

---

### Step 4 — The Next Scaling Problem: Parsing Bloats the Adapter

When mapping grows (units, time alignment, quality flags, parameter renames), adapters become “dump zones.”

```mermaid
flowchart LR
Client --> AP[Controller/gRPC Endpoint]
AP --> App[WeatherService]
App --> A[ThirdPartyAdapter]

A --> External[3rd Party Weather API]
A -->|mapping + conversions + validation\nschema/version handling| Canon[Uniform Response]
```

**Smell**
- Adapter now handles both integration orchestration **and** transformation policy.
- Multiple reasons to change the same class.

---

### Step 5 — Introduce a Translator (Parser/Mapper) Collaborator

Adapter orchestrates IO; translator handles data-shape conversion and normalization.

```mermaid
classDiagram
direction LR

class Client
class ForecastController
class WeatherService

class IForecastProvider {
  <<interface>>
  +GetForecastAsync(...)
}

class ThirdPartyAdapter {
  -ThirdPartyWeatherClient _client
  -ITranslator~ThirdPartyDto,Forecast~ _translator
}

class ThirdPartyWeatherClient
class ITranslator~ThirdPartyDto,Forecast~ {
  <<interface>>
  +Translate(dto) Forecast
}
class ThirdPartyToForecastTranslator

Client --> ForecastController
ForecastController --> WeatherService
WeatherService --> IForecastProvider

ThirdPartyAdapter ..|> IForecastProvider
ThirdPartyAdapter --> ThirdPartyWeatherClient : calls
ThirdPartyAdapter --> ITranslator~ThirdPartyDto,Forecast~ : delegates
ThirdPartyToForecastTranslator ..|> ITranslator~ThirdPartyDto,Forecast~
```

**Result**
- Adapter stays small and integration-focused
- Translator becomes pure, testable, reusable, versionable

---

### Step 6 — Optional: Runtime Translator Selection via Resolver

Only when needed (v1/v2 payloads, content-type variation, schema flags).

```mermaid
flowchart LR
Client --> AP[Controller/gRPC Endpoint]
AP --> App[WeatherService]
App --> A[ThirdPartyAdapter]

A --> DTO[ThirdPartyDto]
A --> R[ITranslatorResolver]
R --> T1[Translator V1]
R --> T2[Translator V2]

T1 --> Canon[Uniform Response]
T2 --> Canon
```

**Note**
- Prefer a resolver over putting Factory Method complexity inside the adapter.

---

## ✅ The "Refactored": Life With Adapter (Server-side / Core)

### Domain Contract (Uniform Interface)

```csharp
public record Point(double Latitude, double Longitude);
public record Forecast(DateTime Time, double TemperatureCelsius, string Source);

public interface IForecastProvider
{
    Task<Forecast> GetForecastAsync(Point location, DateTime at, CancellationToken ct = default);
}
```

### Legacy Provider (Incompatible Interface)

```csharp
public class LegacyGridModel
{
    public Task<LegacyGridResult> QueryGridAsync(double lat, double lon, string timeIso)
        => throw new NotImplementedException();
}

public record LegacyGridResult(string isoTime, double tempKelvin);
```

### Adapter: Legacy → IForecastProvider

```csharp
public sealed class LegacyGridAdapter : IForecastProvider
{
    private readonly LegacyGridModel _legacy;
    private readonly ILogger<LegacyGridAdapter> _log;

    public LegacyGridAdapter(LegacyGridModel legacy, ILogger<LegacyGridAdapter> log)
    {
        _legacy = legacy ?? throw new ArgumentNullException(nameof(legacy));
        _log = log;
    }

    public async Task<Forecast> GetForecastAsync(Point location, DateTime at, CancellationToken ct = default)
    {
        _log.LogDebug("Adapting LegacyGridModel for {lat},{lon} @ {time}",
            location.Latitude, location.Longitude, at);

        var result = await _legacy.QueryGridAsync(location.Latitude, location.Longitude, at.ToString("o"));

        var tempC = result.tempKelvin - 273.15;
        return new Forecast(DateTime.Parse(result.isoTime), tempC, "LegacyGridModel");
    }
}
```

### Third-Party Provider (Different DTOs)

```csharp
public class ThirdPartyWeatherClient
{
    public Task<ThirdPartyDto> GetAsync(string latLon, long epochSeconds, CancellationToken ct = default)
        => throw new NotImplementedException();
}

public record ThirdPartyDto(long ts, double tempF);
```

### Translator: ThirdPartyDto → Forecast

```csharp
public interface ITranslator<TSource, TTarget>
{
    TTarget Translate(TSource source);
}

public sealed class ThirdPartyToForecastTranslator : ITranslator<ThirdPartyDto, Forecast>
{
    public Forecast Translate(ThirdPartyDto dto)
    {
        var time = DateTimeOffset.FromUnixTimeSeconds(dto.ts).UtcDateTime;
        var tempC = (dto.tempF - 32) * 5.0 / 9.0;
        return new Forecast(time, tempC, "ThirdParty");
    }
}
```

### Adapter: Third-Party → IForecastProvider (Delegates translation)

```csharp
public sealed class ThirdPartyAdapter : IForecastProvider
{
    private readonly ThirdPartyWeatherClient _client;
    private readonly ITranslator<ThirdPartyDto, Forecast> _translator;

    public ThirdPartyAdapter(ThirdPartyWeatherClient client, ITranslator<ThirdPartyDto, Forecast> translator)
    {
        _client = client;
        _translator = translator;
    }

    public async Task<Forecast> GetForecastAsync(Point location, DateTime at, CancellationToken ct = default)
    {
        var dto = await _client.GetAsync(
            $"{location.Latitude},{location.Longitude}",
            new DateTimeOffset(at).ToUnixTimeSeconds(),
            ct);

        return _translator.Translate(dto);
    }
}
```

---

## ✅ Client-side Example: Life With Adapter

### Client Access Point (Controller) stays simple

```csharp
[ApiController]
[Route("api/forecast")]
public sealed class ForecastController : ControllerBase
{
    private readonly WeatherService _service;
    public ForecastController(WeatherService service) => _service = service;

    [HttpGet("point")]
    public async Task<ActionResult<Forecast>> GetPoint(
        [FromQuery] double lat,
        [FromQuery] double lon,
        [FromQuery] DateTime at,
        CancellationToken ct)
    {
        var forecast = await _service.GetPointForecast(new Point(lat, lon), at, ct);
        return Ok(forecast);
    }
}
```

### Application Service depends only on abstraction

```csharp
public sealed class WeatherService
{
    private readonly IForecastProvider _provider;
    public WeatherService(IForecastProvider provider) => _provider = provider;

    public Task<Forecast> GetPointForecast(Point p, DateTime at, CancellationToken ct = default)
        => _provider.GetForecastAsync(p, at, ct);
}
```

---

## ❌ The "Anti-Pattern": Life Without Adapter (Server-side / Core)

```csharp
public sealed class MonolithicForecastService
{
    public async Task<Forecast> GetForecastAsync(object provider, Point p, DateTime at)
    {
        if (provider is LegacyGridModel legacy)
        {
            var res = await legacy.QueryGridAsync(p.Latitude, p.Longitude, at.ToString("o"));
            return new Forecast(DateTime.Parse(res.isoTime), res.tempKelvin - 273.15, "LegacyGrid");
        }

        if (provider is ThirdPartyWeatherClient third)
        {
            var dto = await third.GetAsync($"{p.Latitude},{p.Longitude}", new DateTimeOffset(at).ToUnixTimeSeconds());
            var tempC = (dto.tempF - 32) * 5.0 / 9.0;
            return new Forecast(DateTimeOffset.FromUnixTimeSeconds(dto.ts).UtcDateTime, tempC, "ThirdParty");
        }

        throw new NotSupportedException("Provider not supported");
    }
}
```

### The Code Smell
- Type checks and branching logic (`if provider is ...`)
- Mapping duplicated across branches
- Central service becomes the bottleneck for changes

### The Tech Lead’s Critique
- **Maintenance tax:** each new provider adds branching and mapping
- **Testing difficulty:** must instantiate concrete types or mock messy seams
- **Fragility:** changes in external DTOs ripple into core service
- **OCP violation:** the central service must be modified for every addition

---

## ❌ Client-side Example: Life Without Adapter

```csharp
[ApiController]
[Route("api/forecast")]
public sealed class ForecastController : ControllerBase
{
    private readonly MonolithicForecastService _svc;
    private readonly object _provider; // leaky abstraction

    public ForecastController(MonolithicForecastService svc, object provider)
    {
        _svc = svc;
        _provider = provider;
    }

    [HttpGet("point")]
    public async Task<ActionResult<Forecast>> GetPoint(double lat, double lon, DateTime at)
    {
        // Controller is now indirectly coupled to all providers
        var forecast = await _svc.GetForecastAsync(_provider, new Point(lat, lon), at);
        return Ok(forecast);
    }
}
```

**What’s wrong**
- Controllers/services are forced to carry “provider identity” around
- Adding providers increases client-side complexity and risk

---

## Real-life Use Cases

### Where Adapter shines in production .NET systems
- **Modular monolith migration:** unify waves/currents/atmospheric behind a stable contract
- **Legacy SDK integration:** wrap vendor classes without changing them
- **Multi-provider strategy:** swap provider implementations without changing client code
- **Version migrations:** keep `IForecastProvider` stable while external providers change
- **Standardizing responses:** internal canonical DTOs (GeoJSON/Open-Meteo-like) maintained centrally

### Your example (from this discussion)
> Adapter pattern is used while converting a monolith into modular monoliths.  
> Waves/Currents/Atmospheric domains needed to be represented uniformly.

That is exactly the “textbook real world” use of Adapter.

---

## Correct Dependency Injection Strategy

### The lifetime rule (Tech Lead rule)
> The adapter lifetime must not be longer than the shortest lifetime of its dependencies.

### Why “Singleton by default” is risky
Singleton is only safe when the service is:
- stateless
- thread-safe
- not dependent on scoped services (DbContext)

### Recommended DI wiring for the example

```csharp
// HTTP client should typically be factory-managed
builder.Services.AddHttpClient<ThirdPartyWeatherClient>();

// Translators are pure => singleton is ideal
builder.Services.AddSingleton<ITranslator<ThirdPartyDto, Forecast>, ThirdPartyToForecastTranslator>();

// Adapters are orchestration & usually stateless => transient is safe default
builder.Services.AddTransient<IForecastProvider, ThirdPartyAdapter>();

// App service (often scoped in web apps, but can be transient too)
builder.Services.AddScoped<WeatherService>();
```

### When adapter must be Scoped
If adapter depends on:
- DbContext
- other scoped per-request services

Then adapter must be scoped:

```csharp
builder.Services.AddScoped<IForecastProvider, DbBackedForecastAdapter>();
```

---

## Parsing Placement: Adapter vs Translator (What we concluded)

### Recommended placement
- **Adapter:** orchestration (call third party, handle errors, cancellation, logging context)
- **Translator/Parser:** mapping & normalization policy (DTO shape conversions, units, naming, missing-value handling)
- **Optional Validator:** deeper domain validation

### Why parsing *inside* adapter tends to drift away from Clean Architecture and SOLID
Not because it’s forbidden, but because it commonly grows into:
- conversions
- normalization rules
- schema/version handling
- missing-data policy

That creates multiple reasons to change the adapter (**SRP pressure**) and couples policy to vendor DTO shapes (**DIP pressure**).

### When parsing inside adapter is acceptable
If mapping is genuinely tiny and stable (2–5 simple fields), it’s fine.  
Once it grows or needs dedicated tests, extract it to a translator.

---

## “Is there a translator/parser pattern?”

Patterns and concepts that cover this role:
- **Anti-Corruption Layer (DDD):** shield your domain from foreign models
- **Data Mapper (Fowler):** map between models (DTO → canonical)
- **Strategy:** choose among translation algorithms (v1/v2, schemas)
- **Chain of Responsibility:** parsing/normalization pipeline
- **Interpreter:** real parsing of grammars (rare for DTO mapping)

In most enterprise .NET systems: **Adapter + Data Mapper/ACL** is the common combination.

---

## “How does adapter know which translator to create?” (Best solution)

### Best default solution (no runtime selection)
✅ **Constructor-inject a single translator**, let DI choose the implementation.

**Why**
- Adapter stays SRP-clean
- No factories inside adapter
- Easy to test

### If runtime selection is required
✅ Inject a **resolver**, not Factory Method complexity inside adapter.

Example idea:
- `ITranslatorResolver` chooses translator based on schema version flags, content type, etc.

---

## Decision Checklist: Is Adapter suitable?

- Do you have incompatible interfaces (legacy SDKs, third parties, mismatched signatures)?
- Do you need a stable internal contract while providers evolve independently?
- Do you want to isolate integration details from app/domain logic?
- Will you add more providers/domains over time?
- Do you need to unit test your application logic without spinning up real providers?

If most answers are “yes”, Adapter is a strong fit.

---

## Patterns Used Together

- **Factory / Abstract Factory:** build/select the correct adapter per domain/provider
- **Strategy:** adapters often *are* strategies behind `IForecastProvider`
- **Decorator:** add caching/retries/metrics around `IForecastProvider` without changing adapters
- **Facade:** expose a simplified aggregated forecast API across multiple adapters
- **Anti-Corruption Layer / Data Mapper:** translation layer to protect your canonical model

---

## SOLID Impact

- **SRP:** Adapter orchestrates; Translator transforms. Each has one reason to change.
- **OCP:** Add new adapters/translators without modifying existing client code.
- **ISP:** Clients depend on a small `IForecastProvider`, not large vendor APIs.
- **DIP:** High-level modules depend on abstractions (`IForecastProvider`, `ITranslator`), not concrete providers.
- **LSP (note):** Works when adapters honor interface semantics consistently.

---

## Key Takeaway for Tech Leads

Adapter is a high-leverage pattern for integration-heavy systems:
- It **decouples** clients from incompatible provider interfaces
- It improves **testability** and reduces **maintenance tax**
- With a Translator/Mapper, it scales cleanly as normalization rules grow
- DI wiring becomes your composition control point
- It’s especially powerful in **modular monolith migrations** (waves/currents/atmospheric → uniform contract)

---

## Appendix: Complete End-to-End Diagram (Client + Access Point + DI + Translator)

```mermaid
flowchart LR
Client[Client / UI / External App] --> AP[Client Access Point\nController/gRPC Endpoint]
AP --> App[Application Service\nWeatherService]
App -->|depends on| IFP[IForecastProvider]

subgraph DI["DI Container"]
  IFP --> A[ThirdPartyAdapter]
  A --> C[ThirdPartyWeatherClient\n(AddHttpClient)]
  A --> T[ThirdPartyToForecastTranslator\n(Singleton)]
end

C --> External[Third Party API]
T --> Canon[Uniform Response]
```

