# e-Carbon Sistem Açıklaması (Türkçe)

## 1. Genel Bakış

e-Carbon API, karbon emisyonlarını hesaplama, saklama ve raporlama için geliştirilmiş bir sistemdir. **Vertical Slice Architecture (Dikey Dilim Mimarisi)** kullanılarak yapılmıştır.

## 2. Mimarinin Temel Taşları

### 2.1 Program.cs - Uygulamanın Başlangıç Noktası

```csharp
var builder = WebApplication.CreateBuilder(args);

// Swagger ve API dokümantasyonu
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HTTP Context erişimi (Audit için gerekli)
builder.Services.AddHttpContextAccessor();

// Veritabanı bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// MediatR yapılandırması - Pipeline davranışları
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuditingBehavior<,>));
});

// FluentValidation - Girdi doğrulama
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
```

**Önemli Nokta:** Burada iki kritik pipeline behavior ekleniyor:
1. **ValidationBehavior** - Her request'ten önce validasyon çalışır
2. **AuditingBehavior** - Her command sonrası audit log kaydı oluşturur

### 2.2 Middleware Zinciri

```csharp
var app = builder.Build();

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();      // Hata yakalama
app.UseMiddleware<RequestTimingMiddleware>();      // İstek süresi ölçümü
app.UseCors("AllowAll");

// Uygulama başlatılırken veritabanını güncelle
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
```

## 3. Veritabanı Katmanı (DbContext)

### 3.1 AppDbContext

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // 8 adet DbSet (Tablo)
    public DbSet<Company> Companies { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<EmissionFactor> EmissionFactors { get; set; }
    public DbSet<ActivityRecord> ActivityRecords { get; set; }
    public DbSet<MonthlySnapshot> MonthlySnapshots { get; set; }
    public DbSet<SnapshotItem> SnapshotItems { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tüm konfigürasyonları otomatik bul ve uygula
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
```

### 3.2 Entity Konfigürasyonu (Örnek: Company)

```csharp
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");  // Tablo adı
        
        builder.HasKey(x => x.Id);     // Primary key
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("UX_companies_name");
        
        // Soft delete filtresi - Silinen kayıtları otomatik filtrele
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
```

## 4. Domain (Varlık) Modelleri

### 4.1 Company (Şirket)

```csharp
public class Company
{
    public Guid Id { get; set; }                    // UUID Primary Key
    public string Name { get; set; } = string.Empty; // Şirket adı
    public DateTime CreatedAt { get; set; }         // Oluşturulma zamanı
    public DateTime UpdatedAt { get; set; }         // Güncellenme zamanı
    public bool IsDeleted { get; set; }             // Soft delete flag
    
    // Navigation Properties (İlişkiler)
    public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
    public ICollection<MonthlySnapshot> MonthlySnapshots { get; set; } = new List<MonthlySnapshot>();
}
```

### 4.2 MonthlySnapshot (Aylık Dondurulmuş Veri)

```csharp
public class MonthlySnapshot
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Month { get; set; } = string.Empty;  // "2024-01" formatında
    public SnapshotStatus Status { get; set; }         // Draft veya Frozen
    public decimal Scope1TotalKg { get; set; }         // Scope 1 toplam emisyon
    public decimal Scope2TotalKg { get; set; }         // Scope 2 toplam emisyon
    public decimal TotalKg { get; set; }               // Toplam emisyon
    public DateTime CreatedAt { get; set; }
    public DateTime? FrozenAt { get; set; }            // Dondurulma zamanı (opsiyonel)
    
    // Navigation Properties
    public Company Company { get; set; } = null!;
    public ICollection<SnapshotItem> SnapshotItems { get; set; } = new List<SnapshotItem>();
    public Report? Report { get; set; }
}
```

### 4.3 AuditLog (Denetim Kaydı) - Değiştirilemez!

```csharp
public class AuditLog
{
    public Guid Id { get; set; }
    public string Actor { get; set; } = string.Empty;      // Kim yaptı
    public string Action { get; set; } = string.Empty;     // Ne yaptı (Create, Update, Delete)
    public string EntityType { get; set; } = string.Empty; // Hangi entity
    public Guid EntityId { get; set; }                     // Entity ID'si
    public string Summary { get; set; } = string.Empty;    // Özet bilgi
    public DateTime CreatedAt { get; set; }                // Zaman damgası
}
```

## 5. Pipeline Behaviors (Otomatik İşlemler)

### 5.1 ValidationBehavior - Otomatik Doğrulama

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Eğer validator yoksa, direkt devam et
        if (!_validators.Any())
        {
            return await next();
        }

        // Tüm validator'ları çalıştır
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Hataları topla
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // Hata varsa exception fırlat
        if (failures.Any())
        {
            throw new Exceptions.ValidationException(failures);
        }

        // Hata yoksa bir sonraki adıma geç
        return await next();
    }
}
```

**Ne İşe Yarar?** Her request geldiğinde otomatik olarak FluentValidation kurallarını çalıştırır. Eğer validasyon hatası varsa, işlem durur ve hata döner.

### 5.2 AuditingBehavior - Otomatik Loglama

```csharp
public class AuditingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Önce işlemi çalıştır
        var response = await next();
        
        // Sadece Command'leri audit et (veri değiştiren işlemler)
        var requestName = typeof(TRequest).Name;
        if (requestName.EndsWith("Command"))
        {
            await AuditActionAsync(requestName, request, cancellationToken);
        }

        return response;
    }

    private async Task AuditActionAsync(string actionName, object request, CancellationToken ct)
    {
        // Kimin yaptığını bul
        var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
        
        // Audit kaydı oluştur
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            Actor = user,
            Action = actionName.Replace("Command", ""),
            EntityType = request.GetType().Name.Replace("Command", ""),
            EntityId = ExtractEntityId(request),
            Summary = $"Executed {actionName}",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.AuditLogs.Add(auditLog);
        await _dbContext.SaveChangesAsync(ct);
    }
}
```

**Ne İşe Yarar?** Veri değiştiren her işlem (Create, Update, Delete) sonrası otomatik olarak `AuditLog` tablosuna kayıt atar. Bu sayede **kim, ne zaman, ne yaptı** bilgisi tutulur.

## 6. Middleware - Hata Yönetimi

### 6.1 ErrorHandlingMiddleware

```csharp
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);  // Bir sonraki middleware'e git
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);  // Hatayı yakala ve işle
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;  // 400
                result = JsonSerializer.Serialize(new { errors = validationException.Errors });
                break;
            case NotFoundException _:
                code = HttpStatusCode.NotFound;    // 404
                result = JsonSerializer.Serialize(new { error = exception.Message });
                break;
            default:
                result = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
```

**Ne İşe Yarar?** Tüm uygulama genelinde hataları yakalar ve uygun HTTP response'ları döner. Örneğin:
- Validasyon hatası → 400 Bad Request
- Kayıt bulunamadı → 404 Not Found
- Beklenmedik hata → 500 Internal Server Error

## 7. Sistem Akışı - Nasıl Çalışır?

### Adım 1: Request Gelir
```
POST /api/companies
{
    "name": "Acme Corp"
}
```

### Adım 2: ValidationBehavior Çalışır
- FluentValidation kurallarını kontrol eder
- Eğer hata varsa → 400 Bad Request döner
- Hata yoksa → devam eder

### Adım 3: Handler Çalışır (Örnek: CreateCompanyHandler)
- İş mantığını çalıştırır
- Veritabanına kayıt atar
- Response oluşturur

### Adım 4: AuditingBehavior Çalışır
- "CreateCompanyCommand" adını görür (Command ile bitiyor)
- AuditLog tablosuna kayıt atar:
```json
{
    "actor": "system",
    "action": "CreateCompany",
    "entityType": "CreateCompany",
    "entityId": "guid-degeri",
    "summary": "Executed CreateCompanyCommand",
    "createdAt": "2024-01-15T10:30:00Z"
}
```

### Adım 5: Response Döner
```
201 Created
{
    "id": "guid-degeri",
    "name": "Acme Corp",
    "createdAt": "2024-01-15T10:30:00Z"
}
```

## 8. Özet: Mimari Avantajları

1. **Vertical Slice Architecture**: Her özellik kendi klasöründe (Auth, Companies, Facilities vb.). Bakımı kolay.

2. **Pipeline Behaviors**: 
   - Validasyon otomatik
   - Audit log otomatik
   - Kod tekrarı yok

3. **Middleware**:
   - Merkezi hata yönetimi
   - Request süresi ölçümü

4. **Entity Framework**:
   - Code-first migration
   - Soft delete desteği (IsDeleted flag)
   - Otomatik index'ler

5. **Immutable Audit**:
   - Tüm işlemler kaydedilir
   - Silinemez, değiştirilemez

Bu mimari sayesinde kod **temiz**, **test edilebilir** ve **ölçeklenebilir** olur.

---

## Başlangıç

### 1. Veritabanını Başlat
```bash
docker-compose up -d
```

### 2. Uygulamayı Çalıştır
```bash
cd src/eCarbon.Api
dotnet run
```

### 3. Erişim Noktaları
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **pgAdmin**: http://localhost:5050 (admin@ecarbon.com / admin123)

---

**Not**: Bu doküman DevelopmentPlan.md'nin Türkçe özetidir. Detaylı geliştirme planı için DevelopmentPlan.md dosyasına bakınız.