# QR Code-Based Attendance System - Complete Production Architecture Plan

> **System**: University Faculty QR Attendance System  
> **Backend**: ASP.NET Core Web API (.NET 8+)  
> **Database**: SQL Server 2019+  
> **Frontend**: Flutter/React Native (Mobile), React/Angular (Web)  
> **Users**: Students, Lecturers, Admins

---

## 1. SYSTEM ARCHITECTURE

### 1.1 Architecture Pattern: Clean Architecture + CQRS

**Why Clean Architecture:**
- Separation of concerns (Domain, Application, Infrastructure, Presentation)
- Dependency inversion for testability
- Independent of UI, Database, or Frameworks
- Easy to switch from QR to NFC/Biometric later

**Why CQRS (Command Query Responsibility Segregation):**
- Read/Write workloads have different characteristics
- Attendance recording (write-heavy) vs. Reports (read-heavy)
- Scales independently

---

### 1.2 Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                         PRESENTATION LAYER                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │   Mobile     │  │   Web App    │  │   Admin      │          │
│  │   (Student)  │  │   (Doctor)   │  │   Dashboard  │          │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘          │
│         │                  │                  │                    │
│         └──────────────────┼──────────────────┘                    │
│                            │                                       │
│                    ┌───────▼────────┐                             │
│                    │  API Controllers│                             │
│                    │  (Auth, QR,     │                             │
│                    │   Attendance)   │                             │
│                    └───────┬────────┘                             │
└────────────────────────────┼─────────────────────────────────────┘
                             │
┌────────────────────────────┼─────────────────────────────────────┐
│  APPLICATION LAYER        │                                     │
│  ┌────────────────────────▼─────────────────────────────┐        │
│  │  Services (AttendanceService, QRService, AuthService)│        │
│  │  DTOs, Interfaces, Validators (FluentValidation)     │        │
│  │  CQRS Commands/Queries (MediatR)                    │        │
│  └────────────────────────┬────────────────────────────┘        │
└────────────────────────────┼─────────────────────────────────────┘
                             │
┌────────────────────────────┼─────────────────────────────────────┐
│  DOMAIN LAYER (Core)     │                                     │
│  ┌────────────────────────▼─────────────────────────────┐        │
│  │  Entities (User, Course, Session, Attendance)        │        │
│  │  Enums (UserRole, AttendanceStatus, QRType)          │        │
│  │  Interfaces (IRepository, IQrGenerator)              │        │
│  │  Domain Events (AttendanceMarkedEvent)               │        │
│  └────────────────────────┬────────────────────────────┘        │
└────────────────────────────┼─────────────────────────────────────┘
                             │
┌────────────────────────────┼─────────────────────────────────────┐
│  INFRASTRUCTURE LAYER     │                                     │
│  ┌────────────────────────▼─────────────────────────────┐        │
│  │  Repositories (EF Core, Dapper for reads)           │        │
│  │  DbContext, Migrations, Seed Data                  │        │
│  │  External Services (QR Generator, Email, SMS)       │        │
│  │  Identity, JWT, HMAC Token Services                │        │
│  └────────────────────────┬────────────────────────────┘        │
└────────────────────────────┼─────────────────────────────────────┘
                             │
                    ┌────────▼────────┐
                    │  SQL Server DB  │
                    │  (Tables, SP,   │
                    │   Indexes)      │
                    └─────────────────┘
```

---

### 1.3 Layer Responsibilities

| Layer | Responsibility | Technologies |
|-------|-----------------|----------------|
| **Domain** | Enterprise business logic, entities, rules | C# classes, no dependencies |
| **Application** | Use cases, orchestration, DTOs, validation | MediatR, FluentValidation, AutoMapper |
| **Infrastructure** | External concerns: DB, QR, Email, File Storage | EF Core, QR Lib, SMTP, Cloud Storage |
| **Presentation** | API endpoints, request handling, auth | ASP.NET Core, JWT, Swagger |

---

### 1.4 Project Structure

```
UniversityQRAttendance/
├── UniversityQRAttendance.Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Student.cs
│   │   ├── Lecturer.cs
│   │   ├── Course.cs
│   │   ├── Session.cs
│   │   ├── Attendance.cs
│   │   └── QrCode.cs
│   ├── Enums/
│   │   ├── UserRole.cs
│   │   ├── AttendanceStatus.cs
│   │   └── QrCodeType.cs
│   ├── Interfaces/
│   │   ├── IRepository.cs
│   │   ├── IQrCodeGenerator.cs
│   │   └── IAttendanceService.cs
│   └── Events/
│       └── AttendanceMarkedEvent.cs
│
├── UniversityQRAttendance.Application/
│   ├── DTOs/
│   │   ├── Auth/
│   │   ├── QrCode/
│   │   └── Attendance/
│   ├── Features/
│   │   ├── Attendance/
│   │   │   ├── Commands/
│   │   │   │   ├── MarkAttendanceCommand.cs
│   │   │   │   └── GenerateQrCommand.cs
│   │   │   └── Queries/
│   │   │       ├── GetAttendanceQuery.cs
│   │   │       └── GetSessionReportQuery.cs
│   │   └── Auth/
│   ├── Interfaces/
│   │   ├── IQrValidationService.cs
│   │   └── ICurrentUserService.cs
│   ├── Validators/
│   └── Common/
│       └── MappingProfiles.cs
│
├── UniversityQRAttendance.Infrastructure/
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Configurations/
│   │   └── Migrations/
│   ├── Repositories/
│   ├── Services/
│   │   ├── QrCodeGenerator.cs
│   │   ├── QrValidationService.cs
│   │   └── JwtTokenService.cs
│   └── Identity/
│       └── ApplicationUser.cs
│
└── UniversityQRAttendance.API/
    ├── Controllers/
    │   ├── v1/
    │   │   ├── AuthController.cs
    │   │   ├── QrController.cs
    │   │   ├── AttendanceController.cs
    │   │   └── AdminController.cs
    │   └── v2/ (future versions)
    ├── Middleware/
    │   ├── ExceptionHandlingMiddleware.cs
    │   └── RateLimitingMiddleware.cs
    ├── Filters/
    │   └── ValidateModelAttribute.cs
    ├── Program.cs
    └── appsettings.json
```

---

## 2. CORE FEATURES

### 2.1 QR-Based Attendance Workflow

```
┌─────────────┐          ┌─────────────┐          ┌─────────────┐
│   Lecturer  │          │   System    │          │   Student   │
│   (Web)     │          │   (API)    │          │  (Mobile)   │
└──────┬──────┘          └──────┬──────┘          └──────┬──────┘
       │                        │                        │
       │ 1. Create Session      │                        │
       │────(Course, Date, ────>│                        │
       │   Time, Duration)       │                        │
       │                        │                        │
       │ 2. Generate QR         │                        │
       │<──(Encrypted QR ───────│                        │
       │   Payload + HMAC)      │                        │
       │                        │                        │
       │ 3. Display QR          │                        │
       │    (Projector/Screen)  │                        │
       │                        │                        │
       │                        │ 4. Scan QR             │
       │                        │<──(Student Scans)─────│
       │                        │                        │
       │                        │ 5. Validate QR         │
       │                        │    + Location          │
       │                        │    + Time              │
       │                        │                        │
       │                        │ 6. Record Attendance   │
       │                        │    (If valid)          │
       │                        │                        │
       │ 7. Confirmation        │                        │
       │<──(Success/Fail)──────│──(Response)──────────>│
       │                        │                        │
```

---

### 2.2 Feature Breakdown

| Feature | Description | Priority |
|---------|-------------|----------|
| **QR Generation** | Generate time-limited, HMAC-signed QR codes | P0 |
| **QR Scanning** | Mobile app scans QR via camera | P0 |
| **Session Management** | Lecturers create attendance sessions | P0 |
| **Attendance Recording** | Mark attendance after validation | P0 |
| **Location Validation** | GPS/WiFi validation (optional) | P1 |
| **Duplicate Prevention** | Prevent multiple scans per session | P0 |
| **Admin Dashboard** | View reports, statistics | P2 |
| **Offline Support** | Cache QR data when offline | P1 |
| **Bulk Operations** | Mark attendance for multiple students | P2 |

---

### 2.3 Session (Lecture) Creation Flow

**Step 1: Lecturer Creates Session**
```http
POST /api/v1/sessions
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

{
  "courseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "date": "2026-04-24",
  "startTime": "10:00",
  "endTime": "11:30",
  "qrValiditySeconds": 300,
  "location": {
    "latitude": 30.0444,
    "longitude": 31.2357,
    "radiusMeters": 100
  }
}
```

**Step 2: System Creates Session**
- Generate SessionId (GUID)
- Calculate QR expiration (Now + validitySeconds)
- Generate QR Payload (see Section 3)
- Sign with HMAC-SHA256
- Store session in DB

**Step 3: Return QR Data**
```json
{
  "sessionId": "550e8400-e29b-41d4-a716-446655440000",
  "qrPayload": "eyJzZXNzaW9uSWQiOiI1NTBl...",
  "qrCodeBase64": "iVBORw0KGgoAAAANSUhEUgAA...",
  "expiresAt": "2026-04-24T10:05:00Z",
  "validitySeconds": 300
}
```

---

## 3. QR CODE DESIGN (CRITICAL)

### 3.1 QR Code Payload Structure

**Option Comparison:**

| Type | Payload | Pros | Cons | Security |
|------|---------|------|------|----------|
| **Static** | `{ "courseId": "123" }` | Simple | Replay attacks, sharing | ❌ LOW |
| **Time-Limited** | `{ "sessionId": "456", "exp": 1234567890 }` | Prevents reuse after expiry | Device clock skew issues | ⚠️ MEDIUM |
| **Dynamic (Rotating)** | `{ "sessionId": "789", "timestamp": 1234567890, "hmac": "abc..." }` | Prevents replay, spoofing | More complex | ✅ HIGH |

**RECOMMENDATION: Dynamic QR with HMAC-SHA256**

---

### 3.2 QR Payload Specification (Final)

```json
{
  "sid": "550e8400-e29b-41d4-a716-446655440000",
  "cid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "ts": 1713956400,
  "exp": 1713956700,
  "nonce": "a1b2c3d4e5",
  "sig": "7f3a9b2c1d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0"
}
```

**Field Explanations:**

| Field | Type | Description | Security Purpose |
|-------|------|-------------|------------------|
| `sid` | GUID | Session ID | Links QR to specific session |
| `cid` | GUID | Course ID | Validates course exists |
| `ts` | long | Unix timestamp of generation | Prevents replay attacks |
| `exp` | long | Expiration timestamp | Time-limited QR |
| `nonce` | string | Random 10-char string | Prevents replay with same timestamp |
| `sig` | string | HMAC-SHA256 signature | Prevents payload tampering |

---

### 3.3 HMAC Signature Generation

**Algorithm:** HMAC-SHA256

**Signing String Construction:**
```
signingString = sid + ":" + cid + ":" + ts + ":" + exp + ":" + nonce
```

**Example:**
```csharp
string signingString = "550e8400-e29b-41d4-a716-446655440000:3fa85f64-5717-4562-b3fc-2c963f66afa6:1713956400:1713956700:a1b2c3d4e5";

// HMAC-SHA256 with SECRET key stored in Azure Key Vault / AWS Secrets Manager
string signature = HMACSHA256(signingString, secretKey);
```

**Why HMAC-SHA256?**
- Fast verification (O(1) vs RSA O(n))
- No reversible encryption (payload isn't encrypted, just signed)
- Key can be rotated easily
- Supported on all platforms (mobile, web)

---

### 3.4 QR Code Generation Process

```csharp
public class QrCodeGenerator : IQrCodeGenerator
{
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContext;

    public async Task<QrCodeDto> GenerateQrCodeAsync(Guid sessionId, Guid courseId)
    {
        // 1. Generate timestamp and expiration
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long expiresAt = timestamp + 300; // 5 minutes validity

        // 2. Generate cryptographically secure nonce
        string nonce = GenerateSecureNonce(10);

        // 3. Construct payload
        var payload = new
        {
            sid = sessionId.ToString(),
            cid = courseId.ToString(),
            ts = timestamp,
            exp = expiresAt,
            nonce = nonce
        };

        // 4. Serialize to JSON
        string jsonPayload = JsonSerializer.Serialize(payload);

        // 5. Calculate HMAC signature
        string signingString = $"{payload.sid}:{payload.cid}:{payload.ts}:{payload.exp}:{payload.nonce}";
        string signature = ComputeHmacSha256(signingString, GetSecretKey());

        // 6. Add signature to payload
        var finalPayload = new
        {
            sid = payload.sid,
            cid = payload.cid,
            ts = payload.ts,
            exp = payload.exp,
            nonce = payload.nonce,
            sig = signature
        };

        // 7. Serialize final payload and encode to Base64URL (safe for QR)
        string finalJson = JsonSerializer.Serialize(finalPayload);
        string base64Payload = Base64UrlEncode(Encoding.UTF8.GetBytes(finalJson));

        // 8. Generate QR Code image (using QRCoder or similar)
        byte[] qrImageBytes = GenerateQrImage(base64Payload);

        return new QrCodeDto
        {
            SessionId = sessionId,
            QrPayload = base64Payload,
            QrCodeBase64 = Convert.ToBase64String(qrImageBytes),
            ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresAt),
            ValiditySeconds = 300
        };
    }

    private string GenerateSecureNonce(int length)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)[..length];
    }

    private string ComputeHmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLower();
    }
}
```

---

### 3.5 Preventing QR Sharing & Spoofing

**Attack Vector 1: QR Code Sharing (Student A shares QR with Student B)**
- **Prevention**: Bind QR to session (sessionId), not student
- **Result**: Any student can scan the same QR, but system validates their identity via JWT

**Attack Vector 2: Replay Attack (Reusing old QR)**
- **Prevention**: `exp` (expiration) check + `nonce` + cache used nonces
- **Result**: Expired QRs are rejected; nonces prevent reuse

**Attack Vector 3: QR Spoofing (Generating fake QR)**
- **Prevention**: HMAC signature verification with server-side secret
- **Result**: Client cannot generate valid signature without secret key

**Attack Vector 4: Payload Tampering (Modifying QR payload)**
- **Prevention**: HMAC signature covers ALL fields
- **Result**: Any modification invalidates the signature

**Attack Vector 5: Screenshot/Photo (Student takes photo of QR)**
- **Prevention**: Short validity (60-300 seconds) + rotating QR (lecturer regenerates every 5 min)
- **Result**: Screenshot becomes invalid quickly

---

### 3.6 QR Rotation Strategy

**For 60-90 minute lectures:**

```
Time    | QR Code #   | Valid From    | Valid To
--------|-------------|---------------|---------------
10:00   | QR-1        | 10:00:00      | 10:04:59
10:05   | QR-2        | 10:05:00      | 10:09:59
10:10   | QR-3        | 10:10:00      | 10:14:59
...      | ...         | ...           | ...
```

**Why rotate every 5 minutes?**
- Limits exposure window if QR is shared
- Forces students to scan during class (not after)
- Balances UX (not too frequent) vs Security (not too long)

**Implementation:**
```csharp
// Lecturer clicks "Generate New QR" every 5 minutes
// System creates new session with new sessionId
// Old QR automatically expires (exp field)
```

---

## 4. SECURITY DESIGN (VERY IMPORTANT)

### 4.1 Authentication & Authorization

**JWT Token Structure:**
```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",  // User ID
  "email": "student@university.edu",
  "role": "Student",                               // Student | Lecturer | Admin
  "name": "John Doe",
  "studentId": "202312345",                       // For students only
  "department": "Computer Science",
  "jti": "a1b2c3d4-e5f6-7890-abcd-ef1234567890", // JWT ID (for revocation)
  "iat": 1713956400,                              // Issued at
  "exp": 1713960000,                              // Expires (4 hours)
  "nbf": 1713956400                               // Not before
}
```

**JWT Validation Flow:**
```
Mobile App (Student)                    API Server
     │                                     │
     │ 1. Login (email/password)           │
     │────────────────────────────────────>│
     │                                     │
     │ 2. Validate credentials             │
     │    Generate JWT (4h expiry)         │
     │    Return JWT + Refresh Token       │
     │<────────────────────────────────────│
     │                                     │
     │ 3. Scan QR Code                    │
     │    Extract payload                  │
     │    Add JWT to request header        │
     │────────────────────────────────────>│
     │    Authorization: Bearer <JWT>     │
     │                                     │
     │ 4. Validate JWT (signature, exp)   │
     │    Extract User ID, Role           │
     │    Check if user is Student        │
     │                                     │
     │ 5. Process Attendance              │
     │<────────────────────────────────────│
```

---

### 4.2 Preventing Fake Attendance

**Layer 1: JWT Authentication**
- Only authenticated students can mark attendance
- JWT contains `studentId` and `role`
- Prevents anonymous attendance marking

**Layer 2: QR Code Validation (HMAC Verification)**
- Verify QR signature hasn't been tampered with
- Check `exp` (expiration) - reject expired QRs
- Check `nonce` - prevent replay attacks

**Layer 3: Session Validation**
- Verify `sessionId` exists and is active
- Confirm session hasn't ended
- Validate `courseId` matches session's course

**Layer 4: Student Enrollment Validation**
- Check if student is enrolled in the course
- Query: `Enrollments WHERE StudentId = X AND CourseId = Y AND IsActive = true`
- Reject if not enrolled

**Layer 5: Duplicate Prevention**
- Unique constraint: `(StudentId, SessionId)` in Attendance table
- Check: `Attendances WHERE StudentId = X AND SessionId = Y`
- Reject if already marked

**Layer 6: Time Validation**
- System time vs QR `ts` (timestamp)
- Prevent marking attendance before session starts
- Prevent marking after session ends

**Layer 7 (Optional): Location Validation**
- GPS coordinates in request (mobile app)
- Calculate distance from classroom location
- Reject if > 100 meters (configurable)
- Use WiFi SSID/BSSID as secondary validation

---

### 4.3 Preventing QR Reuse & Replay Attacks

**Replay Attack Prevention:**

```csharp
public class QrValidationService : IQrValidationService
{
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _config;

    public async Task<ValidationResult> ValidateQrPayloadAsync(string base64Payload)
    {
        // 1. Decode Base64URL to JSON
        string jsonPayload;
        try
        {
            byte[] bytes = Base64UrlDecode(base64Payload);
            jsonPayload = Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return ValidationResult.Failed("Invalid QR code format");
        }

        // 2. Deserialize JSON
        QrPayloadDto? payload;
        try
        {
            payload = JsonSerializer.Deserialize<QrPayloadDto>(jsonPayload);
        }
        catch
        {
            return ValidationResult.Failed("Malformed QR payload");
        }

        // 3. Verify all required fields exist
        if (string.IsNullOrEmpty(payload.Sid) ||
            string.IsNullOrEmpty(payload.Cid) ||
            payload.Ts == 0 ||
            payload.Exp == 0 ||
            string.IsNullOrEmpty(payload.Nonce) ||
            string.IsNullOrEmpty(payload.Sig))
        {
            return ValidationResult.Failed("Missing required fields in QR");
        }

        // 4. Check expiration
        long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (currentTimestamp > payload.Exp)
        {
            return ValidationResult.Failed("QR code has expired");
        }

        // 5. Check if timestamp is in the future (clock skew attack)
        if (payload.Ts > currentTimestamp + 30) // Allow 30s skew
        {
            return ValidationResult.Failed("Invalid QR timestamp (too far in future)");
        }

        // 6. Verify HMAC signature
        string signingString = $"{payload.Sid}:{payload.Cid}:{payload.Ts}:{payload.Exp}:{payload.Nonce}";
        string expectedSignature = ComputeHmacSha256(signingString, GetSecretKey());

        if (!CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(payload.Sig),
            Encoding.UTF8.GetBytes(expectedSignature)))
        {
            return ValidationResult.Failed("Invalid QR signature (tampered payload)");
        }

        // 7. Check nonce cache (prevent replay)
        string nonceCacheKey = $"nonce_{payload.Nonce}";
        if (_cache.TryGetValue(nonceCacheKey, out _))
        {
            return ValidationResult.Failed("QR code already used (replay attack detected)");
        }

        // 8. Cache the nonce (with expiration matching QR expiry)
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(payload.Exp - payload.Ts));
        _cache.Set(nonceCacheKey, true, cacheOptions);

        // 9. Return success with extracted data
        return ValidationResult.Success(new ValidatedQrData
        {
            SessionId = Guid.Parse(payload.Sid),
            CourseId = Guid.Parse(payload.Cid),
            GeneratedAt = DateTimeOffset.FromUnixTimeSeconds(payload.Ts),
            ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(payload.Exp)
        });
    }
}
```

**Why use MemoryCache for nonces?**
- Fast O(1) lookup
- Automatic expiration (no manual cleanup)
- Suitable for single-server or sticky sessions
- For multi-server: use Redis instead of MemoryCache

---

### 4.4 Session Expiration Strategy

**Short-lived QR (60-300 seconds):**
- QR expires after 5 minutes (configurable)
- Forces students to scan during active class
- Reduces window for QR sharing

**JWT Token (4 hours):**
- Long enough for a day's work
- Short enough to limit damage if compromised
- Refresh token (30 days) for getting new JWT

**Session (Lecture):**
- Session active duration = lecture duration (e.g., 90 minutes)
- Can be extended by lecturer if needed
- Expired sessions reject new attendance markings

**Attendance record:**
- Permanent once recorded (unless excused)
- Soft-delete capability for admins

---

### 4.5 Rate Limiting

**Why Rate Limiting?**
- Prevent brute-force attacks on QR endpoints
- Prevent DoS attacks
- Prevent accidental spam from buggy mobile apps

**Implementation (ASP.NET Core Middleware):**

```csharp
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    // Rate limit: 10 requests per minute per IP
    private const int MAX_REQUESTS_PER_MINUTE = 10;
    private readonly TimeSpan _window = TimeSpan.FromMinutes(1);

    public async Task InvokeAsync(HttpContext context)
    {
        string clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        string cacheKey = $"rate_limit_{clientIp}_{DateTime.UtcNow:yyyyMMddHHmm}";

        int requestCount = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _window;
            return 0;
        });

        if (requestCount >= MAX_REQUESTS_PER_MINUTE)
        {
            _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}", clientIp);
            context.Response.StatusCode = 429; // Too Many Requests
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                retryAfter = _window.TotalSeconds
            });
            return;
        }

        _cache.Set(cacheKey, requestCount + 1, _window);
        await _next(context);
    }
}
```

**Rate Limit Rules:**

| Endpoint | Limit | Window | Justification |
|----------|--------|-------|---------------|
| `POST /api/v1/attendance` | 5 | 1 minute | Students scan once per session |
| `POST /api/v1/auth/login` | 5 | 5 minutes | Prevent brute-force |
| `GET /api/v1/sessions` | 60 | 1 minute | Normal browsing |
| `POST /api/v1/qr/generate` | 10 | 5 minutes | Lecturers generate QR periodically |

---

### 4.6 HTTPS + Certificate Pinning

**Server-Side (API):**
- Enforce HTTPS (redirect HTTP to HTTPS)
- Use TLS 1.2+ (disable older protocols)
- HSTS header: `Strict-Transport-Security: max-age=31536000; includeSubDomains`

**Mobile App (Certificate Pinning):**
```csharp
// Flutter/Dart example
final securityContext = SecurityContext(withTrustedRoots: false);
securityContext.setTrustedCertificatesBytes(trustedCertBytes);

// Pin the public key hash
final pinnedSha256 = "7F3A9B2C1D4E5F6A7B8C9D0E1F2A3B4C5D6E7F8A9B0C1D2E3F4A5B6C7D8E9F0";
```

**Why Certificate Pinning?**
- Prevents Man-in-the-Middle (MITM) attacks
- Even if attacker has fake CA certificate, pinning rejects it

---

## 5. DATABASE DESIGN

### 5.1 Entity Relationship Diagram (ERD)

```
┌────────────────┐       ┌────────────────┐       ┌────────────────┐
│     User      │       │   Student     │       │   Lecturer    │
│───────────────│       │───────────────│       │───────────────│
│ PK: Id (GUID)│       │ PK: UserId    │       │ PK: UserId    │
│    FullName   │◄──────│ FK: UserId    │       │ FK: UserId    │
│    Email      │       │    StudentId  │       │    Title      │
│    PasswordHash│       │    Department │       │    Specializ.  │
│    Role       │       └──────┬────────┘       │    Office      │
│    IsActive   │              │                └──────┬────────┘
└───────┬───────┘              │                       │
        │                      │                       │
        │                      │   ┌────────────────┐  │
        │                      └──>│  Enrollment    │<-┘
        │                          │────────────────│
        │                          │ PK: Id        │
        │                          │    StudentId  │
        │                          │    CourseId   │
        │                          │    EnrolledAt │
        │                          │    IsActive   │
        │                          └──────┬────────┘
        │                                 │
        │                          ┌──────┴────────┐
        │                          │               │
┌───────▼───────┐       ┌────────▼───────┐      │
│    Course     │       │    Session      │      │
│───────────────│       │────────────────│      │
│ PK: Id (GUID)│       │ PK: Id (GUID)  │      │
│    Code       │       │    CourseId    │      │
│    Name       │       │    LecturerId  │      │
│    Department │       │    Date        │      │
│    Credits    │       │    StartTime   │      │
│    IsActive   │       │    EndTime     │      │
└───────┬───────┘       │    QrExpiry    │      │
        │               │    IsActive    │      │
        │               └────────┬───────┘      │
        │                        │               │
        │                        │               │
        │               ┌────────▼───────┐      │
        │               │   Attendance   │<─────┘
        │               │────────────────│
        │               │ PK: Id (GUID) │
        │               │    SessionId  │
        │               │    StudentId  │
        │               │    Status     │
        │               │    ScannedAt  │
        │               │    Latitude   │
        │               │    Longitude  │
        │               │    IpAddress  │
        └──────────────>│    DeviceId   │
                        └────────────────┘
```

---

### 5.2 SQL Table Definitions

**Users Table:**
```sql
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role INT NOT NULL CHECK (Role IN (1, 2, 3)), -- 1=Student, 2=Lecturer, 3=Admin
    PhoneNumber NVARCHAR(20) NULL,
    ProfilePictureUrl NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    LastLoginAt DATETIME2 NULL,

    INDEX IX_Users_Email (Email),
    INDEX IX_Users_Role (Role)
);
```

**Students Table:**
```sql
CREATE TABLE Students (
    UserId UNIQUEIDENTIFIER PRIMARY KEY,
    StudentId NVARCHAR(20) NOT NULL UNIQUE, -- e.g., "202312345"
    Department NVARCHAR(100) NOT NULL,
    EnrollmentYear INT NOT NULL,
    CurrentSemester INT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Students_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
```

**Lecturers Table:**
```sql
CREATE TABLE Lecturers (
    UserId UNIQUEIDENTIFIER PRIMARY KEY,
    Title NVARCHAR(50) NOT NULL, -- "Dr.", "Prof.", etc.
    Specialization NVARCHAR(200) NOT NULL,
    Department NVARCHAR(100) NOT NULL,
    OfficeLocation NVARCHAR(200) NULL,

    CONSTRAINT FK_Lecturers_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
```

**Courses Table:**
```sql
CREATE TABLE Courses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code NVARCHAR(20) NOT NULL UNIQUE, -- e.g., "CS101"
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL,
    Department NVARCHAR(100) NOT NULL,
    Credits INT NOT NULL CHECK (Credits BETWEEN 1 AND 10),
    Semester NVARCHAR(20) NOT NULL, -- "Fall 2025", "Spring 2026"
    AcademicYear NVARCHAR(10) NOT NULL, -- "2025-2026"
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,

    INDEX IX_Courses_Code (Code),
    INDEX IX_Courses_Department (Department),
    INDEX IX_Courses_Semester (Semester, AcademicYear)
);
```

**Enrollments Table:**
```sql
CREATE TABLE Enrollments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    EnrolledAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Enrollments_Students FOREIGN KEY (StudentId) REFERENCES Users(Id),
    CONSTRAINT FK_Enrollments_Courses FOREIGN KEY (CourseId) REFERENCES Courses(Id),
    CONSTRAINT UQ_Enrollments_StudentCourse UNIQUE (StudentId, CourseId),

    INDEX IX_Enrollments_StudentId (StudentId),
    INDEX IX_Enrollments_CourseId (CourseId)
);
```

**Sessions Table:**
```sql
CREATE TABLE Sessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId UNIQUEIDENTIFIER NOT NULL,
    LecturerId UNIQUEIDENTIFIER NOT NULL,
    SessionDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    QrExpiryTime DATETIME2 NOT NULL,
    LocationLatitude DECIMAL(10, 8) NULL,
    LocationLongitude DECIMAL(11, 8) NULL,
    LocationRadius INT NULL DEFAULT 100, -- meters
    IsActive BIT NOT NULL DEFAULT 1,
    IsCancelled BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,

    CONSTRAINT FK_Sessions_Courses FOREIGN KEY (CourseId) REFERENCES Courses(Id),
    CONSTRAINT FK_Sessions_Lecturers FOREIGN KEY (LecturerId) REFERENCES Users(Id),
    CONSTRAINT CHK_Sessions_Time CHECK (EndTime > StartTime),

    INDEX IX_Sessions_CourseId (CourseId),
    INDEX IX_Sessions_LecturerId (LecturerId),
    INDEX IX_Sessions_Date (SessionDate),
    INDEX IX_Sessions_Active (IsActive, SessionDate)
);
```

**Attendance Table:**
```sql
CREATE TABLE Attendance (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SessionId UNIQUEIDENTIFIER NOT NULL,
    StudentId UNIQUEIDENTIFIER NOT NULL,
    Status INT NOT NULL CHECK (Status IN (1, 2, 3, 4)), -- 1=Present, 2=Absent, 3=Late, 4=Excused
    ScannedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    QrPayload NVARCHAR(MAX) NULL, -- Store QR for audit
    Latitude DECIMAL(10, 8) NULL,
    Longitude DECIMAL(11, 8) NULL,
    IpAddress NVARCHAR(50) NULL,
    DeviceId NVARCHAR(200) NULL, -- Mobile device identifier
    IsValid BIT NOT NULL DEFAULT 1, -- For manual override by lecturer

    CONSTRAINT FK_Attendance_Sessions FOREIGN KEY (SessionId) REFERENCES Sessions(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Attendance_Students FOREIGN KEY (StudentId) REFERENCES Users(Id),
    CONSTRAINT UQ_Attendance_SessionStudent UNIQUE (SessionId, StudentId), -- Prevent duplicates

    INDEX IX_Attendance_SessionId (SessionId),
    INDEX IX_Attendance_StudentId (StudentId),
    INDEX IX_Attendance_Status (Status),
    INDEX IX_Attendance_ScannedAt (ScannedAt)
);
```

**QrCodes Table (Audit Log):**
```sql
CREATE TABLE QrCodes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SessionId UNIQUEIDENTIFIER NOT NULL,
    QrPayload NVARCHAR(MAX) NOT NULL, -- Full QR payload (for audit)
    GeneratedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ExpiresAt DATETIME2 NOT NULL,
    IsUsed BIT NOT NULL DEFAULT 0,
    UsedAt DATETIME2 NULL,
    UsedByStudentId UNIQUEIDENTIFIER NULL,

    CONSTRAINT FK_QrCodes_Sessions FOREIGN KEY (SessionId) REFERENCES Sessions(Id),
    CONSTRAINT FK_QrCodes_Students FOREIGN KEY (UsedByStudentId) REFERENCES Users(Id),

    INDEX IX_QrCodes_SessionId (SessionId),
    INDEX IX_QrCodes_ExpiresAt (ExpiresAt)
);
```

---

### 5.3 Database Indexing Strategy

**High-Volume Read Queries:**

| Query | Index | Reason |
|-------|--------|--------|
| Find attendance by session | `IX_Attendance_SessionId` | List all students in a session |
| Find attendance by student | `IX_Attendance_StudentId` | Student's attendance history |
| Find active sessions | `IX_Sessions_Active` | Lecturer views today's sessions |
| Find enrollments by course | `IX_Enrollments_CourseId` | List students in a course |

**Write Optimization:**
- Use SEQUENTIAL GUIDs (NEWSEQUENTIALID()) for PKs to reduce index fragmentation
- Alternatively: Use BIGINT IDENTITY for high-write tables (Attendance)

**Partitioning (Optional for 10,000+ students):**
```sql
-- Partition Attendance table by SessionDate (monthly)
CREATE PARTITION FUNCTION AttendanceByMonth (DATE)
AS RANGE RIGHT FOR VALUES ('2026-01-01', '2026-02-01', ...);

CREATE PARTITION SCHEME AttendanceScheme
AS PARTITION AttendanceByMonth TO (PRIMARY, [Jan2026], [Feb2026], ...);
```

---

## 6. API DESIGN (ASP.NET CORE)

### 6.1 Complete API Endpoint List

#### Authentication Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/api/v1/auth/login` | Login with email/password | Public |
| POST | `/api/v1/auth/refresh` | Refresh JWT token | Public |
| POST | `/api/v1/auth/logout` | Revoke refresh token | Authenticated |
| GET | `/api/v1/auth/me` | Get current user info | Authenticated |

#### Session (Lecture) Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/api/v1/sessions` | Create new session (lecture) | Lecturer, Admin |
| GET | `/api/v1/sessions` | List sessions (filtered) | Lecturer, Admin |
| GET | `/api/v1/sessions/{id}` | Get session details | Lecturer, Admin |
| PUT | `/api/v1/sessions/{id}` | Update session | Lecturer (owner), Admin |
| DELETE | `/api/v1/sessions/{id}` | Cancel session | Lecturer (owner), Admin |
| POST | `/api/v1/sessions/{id}/generate-qr` | Generate new QR for session | Lecturer (owner), Admin |

#### Attendance Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/api/v1/attendance` | Mark attendance (scan QR) | Student |
| GET | `/api/v1/attendance/me` | My attendance history | Student |
| GET | `/api/v1/attendance/session/{sessionId}` | Session attendance list | Lecturer (owner), Admin |
| GET | `/api/v1/attendance/student/{studentId}` | Student's attendance | Lecturer (course), Admin |
| PUT | `/api/v1/attendance/{id}` | Update attendance status | Lecturer (session owner), Admin |
| DELETE | `/api/v1/attendance/{id}` | Delete attendance record | Admin |

#### Admin Endpoints

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/v1/admin/dashboard` | System statistics | Admin |
| GET | `/api/v1/admin/reports/attendance` | Attendance reports | Admin |
| GET | `/api/v1/admin/reports/courses` | Course statistics | Admin |
| GET | `/api/v1/admin/audit-logs` | View audit trail | Admin |

---

### 6.2 Request/Response Examples

**1. Login Request/Response:**

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "student@university.edu",
  "password": "SecurePass123!",
  "deviceId": "android-abc123",
  "rememberMe": false
}
```

```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "accessTokenExpiry": "2026-04-24T18:00:00Z",
    "refreshToken": "7f3a9b2c1d4e5f6a7b8c9d0e1f2a3b4",
    "refreshTokenExpiry": "2026-05-24T14:00:00Z",
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "fullName": "John Doe",
      "email": "student@university.edu",
      "role": "Student",
      "studentId": "202312345",
      "department": "Computer Science"
    }
  }
}
```

**2. Generate QR Request/Response:**

```http
POST /api/v1/sessions/550e8400-e29b-41d4-a716-446655440000/generate-qr
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "qrValiditySeconds": 300
}
```

```json
{
  "success": true,
  "message": "QR code generated successfully",
  "data": {
    "sessionId": "550e8400-e29b-41d4-a716-446655440000",
    "qrPayload": "eyJzaWQiOiI1NTBlODQwMC1lMjliLTQxZ...",
    "qrCodeBase64": "iVBORw0KGgoAAAANSUhEUgAA...",
    "expiresAt": "2026-04-24T10:05:00Z",
    "validitySeconds": 300
  }
}
```

**3. Mark Attendance Request/Response:**

```http
POST /api/v1/attendance
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "qrPayload": "eyJzaWQiOiI1NTBlODQwMC1lMjliLTQxZ...",
  "latitude": 30.0444,
  "longitude": 31.2357,
  "deviceId": "android-abc123",
  "wifiSsid": "University_Campus"
}
```

```json
{
  "success": true,
  "message": "Attendance marked successfully",
  "data": {
    "attendanceId": "7f3a9b2c-1d4e-5f6a-7b8c-9d0e1f2a3b4c",
    "sessionId": "550e8400-e29b-41d4-a716-446655440000",
    "courseName": "CS101 - Introduction to Programming",
    "status": "Present",
    "scannedAt": "2026-04-24T10:02:15Z",
    "locationValid": true
  }
}
```

**Error Response Example:**
```json
{
  "success": false,
  "message": "QR code has expired",
  "errorCode": "QR_EXPIRED",
  "details": {
    "expiredAt": "2026-04-24T10:05:00Z",
    "currentTime": "2026-04-24T10:06:30Z"
  }
}
```

---

### 6.3 Authentication Flow (JWT + Refresh Token)

```csharp
// Program.cs configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
        ClockSkew = TimeSpan.FromSeconds(30)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

// Generate JWT Token
public string GenerateJwtToken(User user)
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Role, user.Role.ToString()),
        new Claim("name", user.FullName)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _config["JWT:Issuer"],
        audience: _config["JWT:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(4),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

---

### 6.4 Error Handling Strategy

**Global Exception Handling Middleware:**

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");

            context.Response.ContentType = "application/json";

            var response = ex switch
            {
                ValidationException validationEx => new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validationEx.Errors,
                    errorCode = "VALIDATION_ERROR"
                },
                UnauthorizedAccessException => new
                {
                    success = false,
                    message = "Unauthorized access",
                    errorCode = "UNAUTHORIZED"
                },
                ArgumentException argEx => new
                {
                    success = false,
                    message = argEx.Message,
                    errorCode = "INVALID_ARGUMENT"
                },
                _ => new
                {
                    success = false,
                    message = "An internal error occurred",
                    errorCode = "INTERNAL_ERROR",
                    requestId = context.TraceIdentifier
                }
            };

            context.Response.StatusCode = ex switch
            {
                ValidationException => 400,
                UnauthorizedAccessException => 401,
                ArgumentException => 400,
                _ => 500
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
```

**Standard Error Response Format:**
```json
{
  "success": false,
  "message": "Human-readable error message",
  "errorCode": "ERROR_CODE_FOR_CLIENT_HANDLING",
  "details": {
    // Additional error details (optional)
  },
  "requestId": "00-abcdef123456-abcdef789012-00" // For tracing
}
```

---

## 7. IMPLEMENTATION DETAILS

### 7.1 Libraries & Tools

| Purpose | Library | Why This? |
|---------|----------|------------|
| **QR Generation** | `QRCoder` (NuGet) | Pure C#, no dependencies, fast, active maintenance |
| **QR Scanning (Mobile)** | `mobile_scanner` (Flutter) / `MLKit` (Native) | Reliable, supports all QR formats |
| **JWT Token** | `System.IdentityModel.Tokens.Jwt` | Official Microsoft library |
| **HMAC Signing** | `System.Security.Cryptography` | Built-in, no extra packages |
| **FluentValidation** | `FluentValidation.AspNetCore` | Clean validation rules, integrates with ASP.NET |
| **AutoMapper** | `AutoMapper.Extensions.Microsoft.DependencyInjection` | DTO ↔ Entity mapping |
| **Entity Framework** | `Microsoft.EntityFrameworkCore.SqlServer` | ORM for SQL Server |
| **Caching** | `Microsoft.Extensions.Caching.Memory` | For nonce cache (use Redis for multi-server) |
| **Logging** | `Serilog.AspNetCore` | Structured logging, sinks to file/DB/Cloud |

---

### 7.2 Code Snippets

#### 7.2.1 Generate QR Code (Full Implementation)

```csharp
using QRCoder;
using System.Security.Cryptography;
using System.Text.Json;

public class QrCodeGenerator : IQrCodeGenerator
{
    private readonly IConfiguration _configuration;

    public async Task<QrCodeDto> GenerateAsync(Guid sessionId, Guid courseId, int validitySeconds = 300)
    {
        // 1. Generate timestamp and expiration
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long expiresAt = timestamp + validitySeconds;

        // 2. Generate cryptographically secure nonce
        string nonce = GenerateNonce();
        if (string.IsNullOrEmpty(nonce))
            throw new InvalidOperationException("Failed to generate nonce");

        // 3. Construct payload
        var payload = new
        {
            sid = sessionId.ToString(),
            cid = courseId.ToString(),
            ts = timestamp,
            exp = expiresAt,
            nonce = nonce
        };

        // 4. Calculate HMAC signature
        string signingString = $"{payload.sid}:{payload.cid}:{payload.ts}:{payload.exp}:{payload.nonce}";
        string signature = ComputeHmacSignature(signingString);

        // 5. Final payload with signature
        var finalPayload = new
        {
            sid = payload.sid,
            cid = payload.cid,
            ts = payload.ts,
            exp = payload.exp,
            nonce = payload.nonce,
            sig = signature
        };

        // 6. Serialize and Base64Url encode
        string jsonPayload = JsonSerializer.Serialize(finalPayload);
        string base64Payload = Base64UrlEncode(jsonPayload);

        // 7. Generate QR Code image
        byte[] qrImageBytes = GenerateQrImage(base64Payload);

        return new QrCodeDto
        {
            SessionId = sessionId,
            QrPayload = base64Payload,
            QrCodeBase64 = Convert.ToBase64String(qrImageBytes),
            ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresAt),
            GeneratedAt = DateTimeOffset.FromUnixTimeSeconds(timestamp),
            ValiditySeconds = validitySeconds
        };
    }

    private string GenerateNonce()
    {
        try
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[10];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("=", "").Replace("+", "").Replace("/", "");
        }
        catch
        {
            return Guid.NewGuid().ToString("N")[..10];
        }
    }

    private string ComputeHmacSignature(string data)
    {
        try
        {
            string? secretKey = _configuration["Security:QrHmacSecret"];
            if (string.IsNullOrEmpty(secretKey))
                throw new InvalidOperationException("QR HMAC secret not configured");

            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            using var hmac = new HMACSHA256(keyBytes);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] hashBytes = hmac.ComputeHash(dataBytes);
            return ConvertToHexString(hashBytes).ToLower();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to compute HMAC signature", ex);
        }
    }

    private byte[] GenerateQrImage(string data)
    {
        try
        {
            using var generator = new QRCodeGenerator();
            using QRCodeData qrData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            return qrCode.GetGraphic(20); // pixels per module
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to generate QR image", ex);
        }
    }

    private string Base64UrlEncode(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        return Convert.ToBase64String(bytes)
            .Replace("=", "")
            .Replace("+", "-")
            .Replace("/", "_");
    }

    private string ConvertToHexString(byte[] bytes)
    {
        return Convert.ToHexString(bytes);
    }
}
```

---

#### 7.2.2 Validate QR Scan (Full Implementation)

```csharp
public class AttendanceService : IAttendanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQrValidationService _qrValidation;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<AttendanceService> _logger;

    public async Task<Result<AttendanceDto>> MarkAttendanceAsync(MarkAttendanceDto dto)
    {
        // 1. Validate QR Payload (HMAC, expiry, nonce)
        var qrValidationResult = await _qrValidation.ValidateAsync(dto.QrPayload);
        if (!qrValidationResult.IsSuccess)
        {
            _logger.LogWarning("QR validation failed: {Message}", qrValidationResult.ErrorMessage);
            return Result<AttendanceDto>.Failure(qrValidationResult.ErrorMessage);
        }

        var qrData = qrValidationResult.Value;

        // 2. Get Session and validate it's active
        var session = await _unitOfWork.Sessions.GetByIdAsync(qrData.SessionId);
        if (session == null || !session.IsActive)
        {
            return Result<AttendanceDto>.Failure("Session not found or inactive");
        }

        // 3. Verify session hasn't ended
        var now = DateTimeOffset.UtcNow;
        if (now > session.EndTime)
        {
            return Result<AttendanceDto>.Failure("Session has already ended");
        }

        // 4. Get current student from JWT
        var studentId = _currentUser.UserId;
        var student = await _unitOfWork.Users.GetByIdAsync(studentId);
        if (student == null || student.Role != UserRole.Student)
        {
            return Result<AttendanceDto>.Failure("Invalid student");
        }

        // 5. Verify student is enrolled in the course
        bool isEnrolled = await _unitOfWork.Enrollments.IsStudentEnrolledAsync(
            studentId, session.CourseId);

        if (!isEnrolled)
        {
            return Result<AttendanceDto>.Failure("Student not enrolled in this course");
        }

        // 6. Check for duplicate attendance
        bool alreadyMarked = await _unitOfWork.Attendance.ExistsAsync(
            a => a.SessionId == qrData.SessionId && a.StudentId == studentId);

        if (alreadyMarked)
        {
            return Result<AttendanceDto>.Failure("Attendance already marked for this session");
        }

        // 7. (Optional) Validate location
        if (dto.Latitude.HasValue && dto.Longitude.HasValue && session.LocationLatitude.HasValue)
        {
            double distance = CalculateDistance(
                dto.Latitude.Value, dto.Longitude.Value,
                session.LocationLatitude.Value, session.LocationLongitude.Value);

            if (distance > (session.LocationRadius ?? 100))
            {
                _logger.LogWarning("Student {StudentId} location invalid: {Distance}m from class",
                    studentId, distance);
                return Result<AttendanceDto>.Failure(
                    $"You are too far from the classroom ({distance:F0}m). Max allowed: {session.LocationRadius ?? 100}m");
            }
        }

        // 8. Determine attendance status (Present/Late)
        var sessionStartTime = session.SessionDate.Add(session.StartTime);
        var status = now > sessionStartTime.AddMinutes(15)
            ? AttendanceStatus.Late
            : AttendanceStatus.Present;

        // 9. Create attendance record
        var attendance = new Attendance
        {
            Id = Guid.NewGuid(),
            SessionId = qrData.SessionId,
            StudentId = studentId,
            Status = status,
            ScannedAt = now,
            QrPayload = dto.QrPayload, // Store for audit
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            IpAddress = _currentUser.IpAddress,
            DeviceId = dto.DeviceId,
            IsValid = true
        };

        await _unitOfWork.Attendance.AddAsync(attendance);
        await _unitOfWork.SaveChangesAsync();

        // 10. Map to DTO and return
        var attendanceDto = new AttendanceDto
        {
            Id = attendance.Id,
            SessionId = attendance.SessionId,
            CourseName = session.Course.Name,
            Status = attendance.Status.ToString(),
            ScannedAt = attendance.ScannedAt,
            LocationValid = dto.Latitude.HasValue
        };

        _logger.LogInformation("Attendance marked: Student {StudentId}, Session {SessionId}, Status {Status}",
            studentId, qrData.SessionId, status);

        return Result<AttendanceDto>.Success(attendanceDto, "Attendance marked successfully");
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula
        const double R = 6371000; // Earth radius in meters
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double degrees) => degrees * Math.PI / 180;
}
```

---

#### 7.2.3 Mobile App QR Scanning (Flutter Example)

```dart
import 'package:mobile_scanner/mobile_scanner.dart';
import 'package:dio/dio.dart';

class QrScanScreen extends StatefulWidget {
  @override
  _QrScanScreenState createState() => _QrScanScreenState();
}

class _QrScanScreenState extends State<QrScanScreen> {
  MobileScannerController cameraController = MobileScannerController();
  final Dio _dio = Dio();
  bool _isProcessing = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Scan Attendance QR')),
      body: Stack(
        children: [
          MobileScanner(
            controller: cameraController,
            onDetect: (capture) {
              if (!_isProcessing) {
                _processQrCode(capture);
              }
            },
          ),
          if (_isProcessing)
            Center(child: CircularProgressIndicator()),
        ],
      ),
    );
  }

  Future<void> _processQrCode(BarcodeCapture capture) async {
    setState(() => _isProcessing = true);
    cameraController.stop();

    try {
      final barcode = capture.barcodes.first;
      final qrPayload = barcode.rawValue;

      if (qrPayload == null) {
        _showError('Invalid QR code');
        return;
      }

      // Get current location
      Position? position;
      try {
        position = await Geolocator.getCurrentPosition();
      } catch (e) {
        print('Location not available: $e');
      }

      // Send to API
      final response = await _dio.post(
        'https://api.university.edu/api/v1/attendance',
        options: Options(
          headers: {
            'Authorization': 'Bearer ${await _getToken()}',
            'Content-Type': 'application/json',
          },
        ),
        data: {
          'qrPayload': qrPayload,
          'latitude': position?.latitude,
          'longitude': position?.longitude,
          'deviceId': await _getDeviceId(),
        },
      );

      if (response.statusCode == 200) {
        _showSuccess('Attendance marked successfully!');
      } else {
        _showError(response.data['message'] ?? 'Failed to mark attendance');
      }
    } catch (e) {
      _showError('Error: $e');
    } finally {
      setState(() => _isProcessing = false);
      cameraController.start();
    }
  }
}
```

---

## 8. EDGE CASES & FAILURE SCENARIOS

### 8.1 Edge Cases Matrix

| Scenario | Detection | Handling | User Message |
|----------|-----------|----------|--------------|
| **Student scans expired QR** | `exp < currentTimestamp` | Reject, return error | "QR code expired at 10:05 AM. Please ask lecturer for new QR." |
| **No internet connection** | Catch `HttpRequestException` | Cache QR payload locally, retry when online | "You appear offline. Attendance will be saved when connection is restored." |
| **Duplicate scan** | Check DB for existing record | Reject duplicate | "Attendance already marked for this session." |
| **Server downtime** | Health check endpoint fails | Show maintenance page, retry logic | "System temporarily unavailable. Retrying..." |
| **Clock skew** | `ts > currentTime + 30s` | Allow 30s tolerance | (None - accept if within tolerance) |
| **Invalid QR format** | JSON parse fails | Reject immediately | "Invalid QR code. Please scan a valid university QR." |
| **Student not enrolled** | Check enrollments table | Reject | "You are not enrolled in this course." |
| **Session cancelled** | `IsCancelled = true` | Reject | "This session has been cancelled by the lecturer." |
| **Late attendance** | `now > StartTime + 15 min` | Mark as "Late" | "Attendance marked as LATE." |
| **QR used twice (replay)** | Nonce cache hit | Reject | "This QR code has already been used." |

---

### 8.2 Offline Support Strategy

**Problem:** Student scans QR while offline.

**Solution: Queue attendance locally, sync when online.**

```csharp
// Mobile App (Flutter) - Offline Queue
class OfflineAttendanceQueue {
  static const String _storageKey = 'offline_attendance_queue';

  static Future<void> enqueue(MarkAttendanceDto attendance) async {
    final prefs = await SharedPreferences.getInstance();
    List<String> queue = prefs.getStringList(_storageKey) ?? [];

    // Add with timestamp
    final item = {
      'payload': attendance.toJson(),
      'queuedAt': DateTime.now().toIso8601String(),
    };
    queue.add(jsonEncode(item));

    await prefs.setStringList(_storageKey, queue);
  }

  static Future<void> syncWhenOnline() async {
    final prefs = await SharedPreferences.getInstance();
    List<String> queue = prefs.getStringList(_storageKey) ?? [];

    if (queue.isEmpty) return;

    for (int i = 0; i < queue.length; i++) {
      try {
        final item = jsonDecode(queue[i]);
        await _sendToServer(item['payload']);

        // Remove from queue after successful sync
        queue.removeAt(i);
        i--;
      } catch (e) {
        print('Sync failed for item $i: $e');
        break; // Stop on first failure (maintain order)
      }
    }

    await prefs.setStringList(_storageKey, queue);
  }
}
```

**Server-Side: Accept delayed attendance (within reason)**
```csharp
// Allow attendance within 30 minutes of QR expiry (grace period)
if (now > qrData.ExpiresAt.AddMinutes(30))
{
    return Result.Failure("Attendance is too old to be recorded (over 30 min past QR expiry)");
}
```

---

### 8.3 Server Downtime Handling

**Circuit Breaker Pattern:**

```csharp
public class CircuitBreaker
{
    private readonly int _failureThreshold = 5;
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(1);
    private int _failureCount;
    private DateTime _lastFailureTime;
    private CircuitState _state = CircuitState.Closed;

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        if (_state == CircuitState.Open)
        {
            if (DateTime.UtcNow - _lastFailureTime > _timeout)
            {
                _state = CircuitState.HalfOpen;
            }
            else
            {
                throw new CircuitBreakerOpenException("Service unavailable");
            }
        }

        try
        {
            var result = await action();
            _failureCount = 0;
            _state = CircuitState.Closed;
            return result;
        }
        catch
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;

            if (_failureCount >= _failureThreshold)
            {
                _state = CircuitState.Open;
            }

            throw;
        }
    }
}
```

---

## 9. SCALABILITY & PERFORMANCE

### 9.1 Handling 10,000+ Students

**Scenario:** University with 10,000 students, 500 courses, 2,000 sessions/day.

**Read/Write Split:**
- **Writes (Attendance recording):** ~2,000 writes/day (actual: ~500/peak hour)
- **Reads (Reports, dashboards):** ~50,000 reads/day

**Strategy: CQRS (Command Query Responsibility Segregation)**

```
┌─────────────────────────────────────────────────────┐
│                   WRITE SIDE                        │
│  (EF Core + SQL Server)                            │
│  - Mark attendance                                 │
│  - Update session                                  │
│  - High consistency (ACID)                         │
└─────────────────────────────────────────────────────┘
                        │
                        │ (Eventual consistency)
                        ▼
┌─────────────────────────────────────────────────────┐
│                   READ SIDE                         │
│  (Dapper + Read-optimized tables or Redis)         │
│  - Get attendance reports                          │
│  - Dashboard queries                              │
│  - Student history                                │
│  - Low latency, denormalized data                  │
└─────────────────────────────────────────────────────┘
```

---

### 9.2 Caching Strategy

| Data | Cache Type | Duration | Rationale |
|------|------------|----------|-----------|
| **QR Nonces** | MemoryCache (or Redis) | QR validity (5 min) | Prevent replay, fast lookup |
| **Session Data** | Redis | Session duration (90 min) | Reduces DB calls per scan |
| **User Profile** | Redis | 1 hour | JWT already has user info, but useful for lookups |
| **Course List** | MemoryCache | 1 day | Rarely changes |
| **Attendance Reports** | Redis | 5 minutes | Stale-while-revalidate |

**Redis Cache Implementation:**
```csharp
public class CacheService : ICacheService
{
    private readonly IDatabase _redis;

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _redis.StringGetAsync(key);
        if (value.IsNull) return default;

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _redis.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(5));
    }

    public async Task RemoveAsync(string key)
    {
        await _redis.KeyDeleteAsync(key);
    }
}
```

---

### 9.3 Database Performance

**Connection Pooling:**
```csharp
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=AttendanceDB;...;Max Pool Size=100;Min Pool Size=10;"
  }
}
```

**Query Optimization:**
```csharp
// Bad: N+1 queries
var sessions = await _context.Sessions.ToListAsync();
foreach (var session in sessions)
{
    session.Course = await _context.Courses.FindAsync(session.CourseId); // N queries
}

// Good: Eager loading
var sessions = await _context.Sessions
    .Include(s => s.Course)
    .Include(s => s.Lecturer)
    .ToListAsync(); // 1 query
```

**Index Usage Monitoring:**
```sql
-- Find missing indexes
SELECT
    dm_mid.statement AS TableName,
    dm_migs.avg_total_user_cost * (dm_migs.avg_user_impact / 100.0) * (dm_migs.user_seeks + dm_migs.user_scans) AS ImprovementMeasure,
    dm_migs.user_seeks,
    dm_migs.user_scans,
    dm_mid.equality_columns,
    dm_mid.inequality_columns
FROM sys.dm_db_missing_index_groups AS dm_mig
INNER JOIN sys.dm_db_missing_index_group_stats AS dm_migs
    ON dm_migs.group_handle = dm_mig.index_group_handle
INNER JOIN sys.dm_db_missing_index_details AS dm_mid
    ON dm_mig.index_handle = dm_mid.index_handle
ORDER BY ImprovementMeasure DESC;
```

---

### 9.4 Load Balancing & Scaling

**Scenario:** 500 students scanning QR simultaneously at 10:00 AM.

**Architecture:**

```
                   ┌─────────────────┐
                   │  Load Balancer  │
                   │  (Azure LB /    │
                   │   nginx)        │
                   └────────┬────────┘
                            │
              ┌─────────────┼─────────────┐
              │             │             │
        ┌─────▼─────┐ ┌────▼─────┐ ┌──▼────────┐
        │ API Srv 1 │ │ API Srv 2│ │ API Srv 3 │
        │ (4 cores) │ │ (4 cores) │ │ (4 cores) │
        └─────┬─────┘ └────┬─────┘ └──┬────────┘
              │             │             │
              └─────────────┼─────────────┘
                            │
                    ┌───────▼───────┐
                    │  Redis Cache  │  (Shared cache for nonces, sessions)
                    └───────┬───────┘
                            │
                    ┌───────▼───────┐
                    │  SQL Server   │
                    │  (Primary)    │
                    └───────┬───────┘
                            │
                    ┌───────▼───────┐
                    │  SQL Server   │
                    │  (Read Replica)│
                    └───────────────┘
```

**Key Changes for Multi-Server:**
1. **Replace MemoryCache with Redis** (shared cache for nonces)
2. **Sticky Sessions** (optional, simpler with Redis)
3. **Database Read Replicas** (offload reporting queries)

---

## 10. FUTURE IMPROVEMENTS

### 10.1 GPS Validation (Location-Based Attendance)

**Implementation:**
```csharp
// Request includes GPS coordinates from mobile app
public class LocationValidator
{
    public bool Validate(GpsCoordinates studentLocation, GpsCoordinates classLocation, int allowedRadiusMeters)
    {
        double distance = Haversine(studentLocation, classLocation);
        return distance <= allowedRadiusMeters;
    }
}
```

**WiFi Validation (Secondary Check):**
```csharp
public class WifiValidator
{
    public bool Validate(string scannedWifiSsid, string authorizedWifiSsid)
    {
        return scannedWifiSsid == authorizedWifiSsid;
    }
}
```

**Why combine GPS + WiFi?**
- GPS can be spoofed (Fake GPS apps)
- WiFi is harder to spoof (requires physical presence near router)
- Combined: Strong location assurance

---

### 10.2 Face Recognition (Biometric Attendance)

**Flow:**
```
1. Student scans QR
2. App prompts for selfie
3. Compare selfie to student ID photo (stored in DB)
4. If match > 90% confidence → Mark attendance
```

**Libraries:**
- **Azure Face API** (Cloud-based)
- **OpenCV + Dlib** (On-device, privacy-preserving)

**Privacy Concerns:**
- Store face embeddings (numerical vectors), NOT actual photos
- Comply with GDPR/CCPA (facial data is sensitive PII)
- Allow students to opt-out (use manual attendance instead)

---

### 10.3 NFC Tags (Alternative to QR)

**Concept:**
- Place NFC sticker on classroom door
- Student taps phone to NFC tag
- Tag contains sessionId (encrypted)
- Same validation flow as QR

**Pros vs QR:**
| Feature | QR Code | NFC Tag |
|---------|----------|----------|
| **Distance** | 10-50 cm | Touch (0 cm) |
| **Sharing** | Can be photographed | Requires physical access |
| **Cost** | Free (generate) | $0.50 per tag |
| **Speed** | 2-3 seconds | < 1 second |
| **Device Needed** | Camera | NFC reader |

**Recommendation:** Use QR as primary, NFC as optional secondary.

---

### 10.4 Analytics Dashboard

**Metrics to Track:**
- Attendance rate by course (line chart)
- Attendance heatmap (by hour/day)
- Student risk prediction (ML model)
- Late arrivals trend
- QR reuse attempts (security metric)

**Technology Stack:**
- **Backend:** ASP.NET Core API returning aggregated data
- **Frontend:** Chart.js / D3.js / Recharts
- **Database:** Pre-aggregated tables (for performance)

**Example Query (Attendance Rate by Course):**
```sql
SELECT
    c.Code AS CourseCode,
    c.Name AS CourseName,
    COUNT(DISTINCT s.Id) AS TotalSessions,
    COUNT(DISTINCT a.StudentId) AS StudentsAttended,
    CAST(COUNT(a.Id) AS FLOAT) / NULLIF(COUNT(DISTINCT s.Id) * COUNT(DISTINCT e.StudentId), 0) * 100 AS AttendanceRate
FROM Courses c
LEFT JOIN Sessions s ON s.CourseId = c.Id
LEFT JOIN Enrollments e ON e.CourseId = c.Id AND e.IsActive = 1
LEFT JOIN Attendance a ON a.SessionId = s.Id AND a.Status = 1
WHERE s.SessionDate >= @StartDate AND s.SessionDate <= @EndDate
GROUP BY c.Id, c.Code, c.Name
ORDER BY AttendanceRate DESC;
```

---

## 11. DEPLOYMENT CHECKLIST

### 11.1 Pre-Production

- [ ] **Security Secrets:** Store HMAC secret in Azure Key Vault / AWS Secrets Manager
- [ ] **HTTPS Only:** Enforce TLS 1.2+ in production
- [ ] **Database:** Run migrations on staging first
- [ ] **Rate Limiting:** Test rate limit thresholds
- [ ] **Logging:** Configure Serilog to write to file + cloud (Application Insights / CloudWatch)
- [ ] **Monitoring:** Set up health checks (`/health` endpoint)
- [ ] **Backup:** Configure SQL Server automated backups

### 11.2 Go-Live

- [ ] **Load Test:** Simulate 10,000 concurrent users
- [ ] **Penetration Test:** Hire security firm to test QR spoofing, JWT attacks
- [ ] **Mobile App:** Publish to App Store / Google Play
- [ ] **User Training:** Train lecturers on QR generation, attendance viewing
- [ ] **Rollback Plan:** Database rollback scripts ready

---

## JUSTIFICATION SUMMARY

| Design Decision | Justification |
|-----------------|----------------|
| **HMAC-SHA256 (not RSA)** | Faster verification, no certificate management, secret can be rotated easily |
| **Short-lived QR (5 min)** | Balances security (not reusable) vs UX (not too frequent) |
| **JWT (not sessions)** | Stateless, works with mobile apps, scalable |
| **Clean Architecture** | Testable, maintainable, framework-agnostic domain |
| **EF Core for writes, Dapper for reads** | EF for complex domain logic, Dapper for fast reads |
| **Redis for shared cache** | Required for multi-server deployment, fast O(1) lookups |
| **SQL Server (not NoSQL)** | Strong consistency needed for attendance records, ACID compliance |
| **FluentValidation** | Clean validation rules, separates validation from business logic |

---

**END OF PLAN**

This plan provides a complete, production-ready QR Attendance System. Every design decision is justified, security is prioritized, and scalability is built-in from day one.
