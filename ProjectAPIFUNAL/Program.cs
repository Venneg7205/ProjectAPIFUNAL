using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using ProjectAPIFUNAL.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = AuthOptions.ISSUER,
        ValidateAudience = true,
        ValidAudience = AuthOptions.AUDIENCE,
        ValidateLifetime = true,
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        ValidateIssuerSigningKey = true
    };
});
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

IServiceCollection serviceCollection = builder.Services.AddDbContext<ProkatContext>(options => options.UseSqlServer(connection));

// ��������� Swagger ����� ���������� �����������������
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Project API 2", Version = "v1" });
});

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapPost("/login", async (User loginData, ProkatContext db) =>
{
    // ����� ������������ �� email � password
    User? person = await db.Users!.FirstOrDefaultAsync(p => p.Email == loginData.Email && p.Password == loginData.Password);
    if (person is null)
    {
        // ���� ������������ �� ������, ���������� ������ �����������
        return Results.Unauthorized();
    }

    // �������� ������ ����������� (claims)
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, person.Email)
    };

    // ��������� JWT ������
    var jwt = new JwtSecurityToken(
        issuer: AuthOptions.ISSUER,
        audience: AuthOptions.AUDIENCE,
        claims: claims,
        expires: DateTime.Now.Add(TimeSpan.FromMinutes(2)),
        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

    // ����������� ������
    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

    // ������������ ������
    var response = new
    {
        access_token = encodedJwt,
        username = person.Email
    };

    return Results.Json(response);
});


// �������� � ���������� ��
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ProkatContext>();
    context.Database.EnsureCreated();

}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project API 2"));
}

app.UseDefaultFiles();
app.UseStaticFiles();
//��������� ���� ������� ��������
app.MapGet("/api/�������", async (ProkatContext db) =>
{
    var ������� = await db.�������s.ToListAsync();
    return Results.Ok(�������);
});


// ���������� ������ �������
app.MapPost("/api/�������", async (ProkatContext db, ������� newClient) =>
{
    db.�������s.Add(newClient);
    await db.SaveChangesAsync();
    return Results.Created($"/api/�������/{newClient.Id�������}", newClient);
});

// ��������� ������ �������
app.MapPut("/api/�������/{id}", async (ProkatContext db, int id, ������� updatedClient) =>
{
    var client = await db.�������s.FindAsync(id);
    if (client == null) return Results.NotFound();

    client.��� = updatedClient.���;
    client.��� = updatedClient.���;
    client.������������ = updatedClient.������������;
    client.����� = updatedClient.�����;
    client.������� = updatedClient.�������;
    client.���������������� = updatedClient.����������������;
    client.���������� = updatedClient.����������;
    client.����������������������� = updatedClient.�����������������������;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// �������� �������
app.MapDelete("/api/�������/{id}", async (ProkatContext db, int id) =>
{
    var client = await db.�������s.FindAsync(id);
    if (client == null) return Results.NotFound();

    db.�������s.Remove(client);
    await db.SaveChangesAsync();
    return Results.Ok(client);
});
app.MapGet("/api/����������", async (ProkatContext db) =>
{
    var ���� = await db.����������s.ToListAsync();
    return Results.Ok(����);
});

// ���������� ������ ����������
app.MapPost("/api/����������", async (ProkatContext db, ���������� newCar) =>
{
    db.����������s.Add(newCar);
    await db.SaveChangesAsync();
    return Results.Created($"/api/����������/{newCar.Id����������}", newCar);
});

// ��������� ������ ����������
app.MapPut("/api/����������/{id}", async (ProkatContext db, int id, ���������� updatedCar) =>
{
    var car = await db.����������s.FindAsync(id);
    if (car == null) return Results.NotFound();

    car.Id����� = updatedCar.Id�����;
    car.�������� = updatedCar.��������;
    car.����������� = updatedCar.�����������;
    car.�������������� = updatedCar.��������������;
    car.��� = updatedCar.���;
    car.������ = updatedCar.������;
    car.���� = updatedCar.����;
    car.����������� = updatedCar.�����������;
    car.���������������� = updatedCar.����������������;
    car.Id�������� = updatedCar.Id��������;
    car.����������� = updatedCar.�����������;
    car.���������������� = updatedCar.����������������;
    car.������������� = updatedCar.�������������;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// �������� ����������
app.MapDelete("/api/����������/{id}", async (ProkatContext db, int id) =>
{
    var car = await db.����������s.FindAsync(id);
    if (car == null) return Results.NotFound();

    db.����������s.Remove(car);
    await db.SaveChangesAsync();
    return Results.Ok(car);
});
app.MapGet("/api/����������������", async (ProkatContext db) =>
{
    var marks = await db.����������������s.ToListAsync();
    return Results.Ok(marks);
});
// ���������� ����� ����� ����������
app.MapPost("/api/����������������", async (ProkatContext db, ���������������� newMark) =>
{
    db.����������������s.Add(newMark);
    await db.SaveChangesAsync();
    return Results.Created($"/api/����������������/{newMark.Id�����}", newMark);
});

// ��������� ������ ����� ����������
app.MapPut("/api/����������������/{id}", async (ProkatContext db, int id, ���������������� updatedMark) =>
{
    var mark = await db.����������������s.FindAsync(id);
    if (mark == null) return Results.NotFound();

    mark.������������ = updatedMark.������������;
    mark.������������������������� = updatedMark.�������������������������;
    mark.�������� = updatedMark.��������;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// �������� ����� ����������
app.MapDelete("/api/����������������/{id}", async (ProkatContext db, int id) =>
{
    var mark = await db.����������������s.FindAsync(id);
    if (mark == null) return Results.NotFound();

    db.����������������s.Remove(mark);
    await db.SaveChangesAsync();
    return Results.Ok(mark);
});
// ��������� ������ ���� �����������
app.MapGet("/api/����������", async (ProkatContext db) =>
{
    return Results.Ok(await db.����������s.ToListAsync());
});

// ���������� ������ ����������
app.MapPost("/api/����������", async (ProkatContext db, ���������� newEmployee) =>
{
    db.����������s.Add(newEmployee);
    await db.SaveChangesAsync();
    return Results.Created($"/api/����������/{newEmployee.Id����������}", newEmployee);
});

// ���������� ������ ����������
app.MapPut("/api/����������/{id}", async (ProkatContext db, int id, ���������� updatedEmployee) =>
{
    var employee = await db.����������s.FindAsync(id);
    if (employee == null) return Results.NotFound();

    // ���������� ����� ����������
    employee.��� = updatedEmployee.��� ?? employee.���;
    employee.������� = updatedEmployee.������� ?? employee.�������;
    employee.��� = updatedEmployee.��� ?? employee.���;
    employee.����� = updatedEmployee.����� ?? employee.�����;
    employee.������� = updatedEmployee.������� ?? employee.�������;
    employee.���������������� = updatedEmployee.���������������� ?? employee.����������������;
    employee.Id��������� = updatedEmployee.Id��������� ?? employee.Id���������;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// �������� ����������
app.MapDelete("/api/����������/{id}", async (ProkatContext db, int id) =>
{
    var employee = await db.����������s.FindAsync(id);
    if (employee == null) return Results.NotFound();

    db.����������s.Remove(employee);
    await db.SaveChangesAsync();
    return Results.Ok(employee);
});
// ��������� ������ ���� ������� �������
app.MapGet("/api/������", async (ProkatContext db) =>
{
    return Results.Ok(await db.������s.ToListAsync());
});

// ���������� ����� ������ �������
app.MapPost("/api/������", async (ProkatContext db, ������ newRental) =>
{
    db.������s.Add(newRental);
    await db.SaveChangesAsync();
    return Results.Created($"/api/������/{newRental.Id����������}", newRental); // ����������� ��������������� �������������
});

// ���������� ������ �������
app.MapPut("/api/������/{id}", async (ProkatContext db, int id, ������ updatedRental) =>
{
    var rental = await db.������s.FindAsync(id);
    if (rental == null) return Results.NotFound();

    // ���������� ����� ������ �������
    rental.����������� = updatedRental.����������� ?? rental.�����������;
    rental.������������� = updatedRental.������������� ?? rental.�������������;
    rental.������������ = updatedRental.������������ ?? rental.������������;
    rental.Id���������� = updatedRental.Id���������� ?? rental.Id����������;
    rental.Id������� = updatedRental.Id������� ?? rental.Id�������;
    rental.Id������1 = updatedRental.Id������1 ?? rental.Id������1;
    rental.Id������2 = updatedRental.Id������2 ?? rental.Id������2;
    rental.Id������3 = updatedRental.Id������3 ?? rental.Id������3;
    rental.����������� = updatedRental.����������� ?? rental.�����������;
    rental.������������� = updatedRental.������������� ?? rental.�������������;
    rental.Id���������� = updatedRental.Id���������� ?? rental.Id����������;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// �������� ������ �������
app.MapDelete("/api/������/{id}", async (ProkatContext db, int id) =>
{
    var rental = await db.������s.FindAsync(id);
    if (rental == null) return Results.NotFound();

    db.������s.Remove(rental);
    await db.SaveChangesAsync();
    return Results.Ok(rental);
});
// ��������� ������ ���� �������������� �����
app.MapGet("/api/��������������������", async (ProkatContext db) =>
{
    return Results.Ok(await db.��������������������s.ToListAsync());
});

// ���������� ����� �������������� ������
app.MapPost("/api/��������������������", async (ProkatContext db, �������������������� newService) =>
{
    db.��������������������s.Add(newService);
    await db.SaveChangesAsync();
    return Results.Created($"/api/��������������������/{newService.Id������}", newService);
});

// ���������� �������������� ������
app.MapPut("/api/��������������������/{id}", async (ProkatContext db, int id, �������������������� updatedService) =>
{
    var service = await db.��������������������s.FindAsync(id);
    if (service == null) return Results.NotFound();

    // ���������� ����� �������������� ������
    service.������������ = updatedService.������������ ?? service.������������;
    service.�������� = updatedService.�������� ?? service.��������;
    service.���� = updatedService.���� ?? service.����;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// �������� �������������� ������
app.MapDelete("/api/��������������������/{id}", async (ProkatContext db, int id) =>
{
    var service = await db.��������������������s.FindAsync(id);
    if (service == null) return Results.NotFound();

    db.��������������������s.Remove(service);
    await db.SaveChangesAsync();
    return Results.Ok(service);
});

app.MapGet("/api/�����������", async (ProkatContext db) =>
{
    return Results.Ok(await db.�����������s.ToListAsync());
});

// ���������� ������ ���������� � ����� ������
app.MapPost("/api/�����������", async (ProkatContext db, ����������� newHR) =>
{
    db.�����������s.Add(newHR);
    await db.SaveChangesAsync();
    return Results.Created($"/api/�����������/{newHR.���}", newHR); // ����������� `���` ��� ����
});

// ���������� ������ ���������� ������ ������
app.MapPut("/api/�����������/{fio}", async (ProkatContext db, string fio, ����������� updatedHR) =>
{
    var hr = await db.�����������s.FindAsync(fio);
    if (hr == null) return Results.NotFound();

    // ���������� ����� ���������� ������ ������
    hr.��� = updatedHR.��� ?? hr.���;
    hr.������������ = updatedHR.������������ ?? hr.������������;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// �������� ���������� ������ ������
app.MapDelete("/api/�����������/{fio}", async (ProkatContext db, string fio) =>
{
    var hr = await db.�����������s.FindAsync(fio);
    if (hr == null) return Results.NotFound();

    db.�����������s.Remove(hr);
    await db.SaveChangesAsync();
    return Results.Ok(hr);
});
// ��������� ������ ���� �������
app.MapGet("/api/������", async (ProkatContext db) =>
{
    return Results.Ok(await db.������s.ToListAsync());
});

// ���������� ������ ������
app.MapPost("/api/������", async (ProkatContext db, ������ newOrder) =>
{
    db.������s.Add(newOrder);
    await db.SaveChangesAsync();
    return Results.Created($"/api/������/{newOrder.Id������}", newOrder);
});

// ���������� ������
app.MapPut("/api/������/{id}", async (ProkatContext db, int id, ������ updatedOrder) =>
{
    var order = await db.������s.FindAsync(id);
    if (order == null) return Results.NotFound();

    // ���������� ����� ������
    order.Id������� = updatedOrder.Id�������;
    order.Id���������� = updatedOrder.Id����������;
    order.���������� = updatedOrder.����������;
    order.����������� = updatedOrder.�����������;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// �������� ������
app.MapDelete("/api/������/{id}", async (ProkatContext db, int id) =>
{
    var order = await db.������s.FindAsync(id);
    if (order == null) return Results.NotFound();

    db.������s.Remove(order);
    await db.SaveChangesAsync();
    return Results.Ok(order);
});
// ��������� ������ ���� ����������� � ���������
app.MapGet("/api/��������", async (ProkatContext db) =>
{
    return Results.Ok(await db.��������s.ToListAsync());
});

// ���������� ������ ���������� � ��������
app.MapPost("/api/��������", async (ProkatContext db, �������� newVehicle) =>
{
    db.��������s.Add(newVehicle);
    await db.SaveChangesAsync();
    return Results.Created($"/api/��������/{newVehicle.��������}", newVehicle);
});

// ���������� ������ ���������� � ���������
app.MapPut("/api/��������/{gosNumber}", async (ProkatContext db, string gosNumber, �������� updatedVehicle) =>
{
    var vehicle = await db.��������s.FindAsync(gosNumber);
    if (vehicle == null) return Results.NotFound();

    // ���������� ����� ����������
    vehicle.�������� = updatedVehicle.�������� ?? vehicle.��������;
    vehicle.��� = updatedVehicle.��� ?? vehicle.���;
    vehicle.����������� = updatedVehicle.����������� ?? vehicle.�����������;
    vehicle.�������������� = updatedVehicle.�������������� ?? vehicle.��������������;
    vehicle.��� = updatedVehicle.��� ?? vehicle.���;
    vehicle.������ = updatedVehicle.������ ?? vehicle.������;
    vehicle.���� = updatedVehicle.���� ?? vehicle.����;
    vehicle.���������������� = updatedVehicle.���������������� ?? vehicle.����������������;
    vehicle.����������� = updatedVehicle.����������� ?? vehicle.�����������;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// �������� ���������� �� ���������
app.MapDelete("/api/��������/{gosNumber}", async (ProkatContext db, string gosNumber) =>
{
    var vehicle = await db.��������s.FindAsync(gosNumber);
    if (vehicle == null) return Results.NotFound();

    db.��������s.Remove(vehicle);
    await db.SaveChangesAsync();
    return Results.Ok(vehicle);
});
// ��������� ������ ���� ����������� � �������
app.MapGet("/api/������������������", async (ProkatContext db) =>
{
    return Results.Ok(await db.������������������s.ToListAsync());
});

// ���������� ������ ���������� � ������
app.MapPost("/api/������������������", async (ProkatContext db, ������������������ newRentalCar) =>
{
    db.������������������s.Add(newRentalCar);
    await db.SaveChangesAsync();
    return Results.Created($"/api/������������������/{newRentalCar.��������}", newRentalCar);
});

// ���������� ������ ���������� � �������
app.MapPut("/api/������������������/{gosNumber}", async (ProkatContext db, string gosNumber, ������������������ updatedRentalCar) =>
{
    var rentalCar = await db.������������������s.FindAsync(gosNumber);
    if (rentalCar == null) return Results.NotFound();

    // ���������� ����� ���������� � �������
    rentalCar.����������� = updatedRentalCar.����������� ?? rentalCar.�����������;
    rentalCar.������������ = updatedRentalCar.������������ ?? rentalCar.������������;
    rentalCar.������������������������� = updatedRentalCar.������������������������� ?? rentalCar.�������������������������;
    rentalCar.�������� = updatedRentalCar.�������� ?? rentalCar.��������;
    rentalCar.��� = updatedRentalCar.��� ?? rentalCar.���;
    rentalCar.������1 = updatedRentalCar.������1 ?? rentalCar.������1;
    rentalCar.������2 = updatedRentalCar.������2 ?? rentalCar.������2;
    rentalCar.������3 = updatedRentalCar.������3 ?? rentalCar.������3;
    rentalCar.������������� = updatedRentalCar.������������� ?? rentalCar.�������������;
    rentalCar.������� = updatedRentalCar.������� ?? rentalCar.�������;
    rentalCar.��� = updatedRentalCar.��� ?? rentalCar.���;
    rentalCar.������� = updatedRentalCar.������� ?? rentalCar.�������;
    rentalCar.���������������� = updatedRentalCar.���������������� ?? rentalCar.����������������;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// �������� ���������� �� �������
app.MapDelete("/api/������������������/{gosNumber}", async (ProkatContext db, string gosNumber) =>
{
    var rentalCar = await db.������������������s.FindAsync(gosNumber);
    if (rentalCar == null) return Results.NotFound();

    db.������������������s.Remove(rentalCar);
    await db.SaveChangesAsync();
    return Results.Ok(rentalCar);
});
app.Run();
public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // �������� ������
    public const string AUDIENCE = "MyAuthClient"; // ����������� ������
    const string KEY = "mysupersecret_secretsecretsecretkey!123"; // ���� ��� ��������
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}