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

// Использую Swagger чтобы посмотреть работоспособность
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
    // Поиск пользователя по email и password
    User? person = await db.Users!.FirstOrDefaultAsync(p => p.Email == loginData.Email && p.Password == loginData.Password);
    if (person is null)
    {
        // Если пользователь не найден, возвращаем ошибку авторизации
        return Results.Unauthorized();
    }

    // Создание списка утверждений (claims)
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, person.Email)
    };

    // Генерация JWT токена
    var jwt = new JwtSecurityToken(
        issuer: AuthOptions.ISSUER,
        audience: AuthOptions.AUDIENCE,
        claims: claims,
        expires: DateTime.Now.Add(TimeSpan.FromMinutes(2)),
        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

    // Кодирование токена
    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

    // Формирование ответа
    var response = new
    {
        access_token = encodedJwt,
        username = person.Email
    };

    return Results.Json(response);
});


// Проверка и заполнение БД
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
//Получение всех списков клиентов
app.MapGet("/api/Клиенты", async (ProkatContext db) =>
{
    var клиенты = await db.Клиентыs.ToListAsync();
    return Results.Ok(клиенты);
});


// Добавление нового клиента
app.MapPost("/api/Клиенты", async (ProkatContext db, Клиенты newClient) =>
{
    db.Клиентыs.Add(newClient);
    await db.SaveChangesAsync();
    return Results.Created($"/api/Клиенты/{newClient.IdКлиента}", newClient);
});

// Изменение данных клиента
app.MapPut("/api/Клиенты/{id}", async (ProkatContext db, int id, Клиенты updatedClient) =>
{
    var client = await db.Клиентыs.FindAsync(id);
    if (client == null) return Results.NotFound();

    client.Фио = updatedClient.Фио;
    client.Пол = updatedClient.Пол;
    client.ДатаРождения = updatedClient.ДатаРождения;
    client.Адрес = updatedClient.Адрес;
    client.Телефон = updatedClient.Телефон;
    client.ПаспортныеДанные = updatedClient.ПаспортныеДанные;
    client.Активность = updatedClient.Активность;
    client.ВодитескоеУдостовирения = updatedClient.ВодитескоеУдостовирения;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Удаление клиента
app.MapDelete("/api/Клиенты/{id}", async (ProkatContext db, int id) =>
{
    var client = await db.Клиентыs.FindAsync(id);
    if (client == null) return Results.NotFound();

    db.Клиентыs.Remove(client);
    await db.SaveChangesAsync();
    return Results.Ok(client);
});
app.MapGet("/api/Автомобили", async (ProkatContext db) =>
{
    var авто = await db.Автомобилиs.ToListAsync();
    return Results.Ok(авто);
});

// Добавление нового автомобиля
app.MapPost("/api/Автомобили", async (ProkatContext db, Автомобили newCar) =>
{
    db.Автомобилиs.Add(newCar);
    await db.SaveChangesAsync();
    return Results.Created($"/api/Автомобили/{newCar.IdАвтомобиля}", newCar);
});

// Изменение данных автомобиля
app.MapPut("/api/Автомобили/{id}", async (ProkatContext db, int id, Автомобили updatedCar) =>
{
    var car = await db.Автомобилиs.FindAsync(id);
    if (car == null) return Results.NotFound();

    car.IdМарки = updatedCar.IdМарки;
    car.ГосНомер = updatedCar.ГосНомер;
    car.НомерКузова = updatedCar.НомерКузова;
    car.НомерДвигателя = updatedCar.НомерДвигателя;
    car.Год = updatedCar.Год;
    car.Пробег = updatedCar.Пробег;
    car.Цена = updatedCar.Цена;
    car.ЦенаПроката = updatedCar.ЦенаПроката;
    car.ДатаПоследнегоТо = updatedCar.ДатаПоследнегоТо;
    car.IdМеханика = updatedCar.IdМеханика;
    car.Особенности = updatedCar.Особенности;
    car.ОтметкаОвозврате = updatedCar.ОтметкаОвозврате;
    car.ОтметкаОброни = updatedCar.ОтметкаОброни;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Удаление автомобиля
app.MapDelete("/api/Автомобили/{id}", async (ProkatContext db, int id) =>
{
    var car = await db.Автомобилиs.FindAsync(id);
    if (car == null) return Results.NotFound();

    db.Автомобилиs.Remove(car);
    await db.SaveChangesAsync();
    return Results.Ok(car);
});
app.MapGet("/api/МаркиАвтомобилей", async (ProkatContext db) =>
{
    var marks = await db.МаркиАвтомобилейs.ToListAsync();
    return Results.Ok(marks);
});
// Добавление новой марки автомобиля
app.MapPost("/api/МаркиАвтомобилей", async (ProkatContext db, МаркиАвтомобилей newMark) =>
{
    db.МаркиАвтомобилейs.Add(newMark);
    await db.SaveChangesAsync();
    return Results.Created($"/api/МаркиАвтомобилей/{newMark.IdМарки}", newMark);
});

// Изменение данных марки автомобиля
app.MapPut("/api/МаркиАвтомобилей/{id}", async (ProkatContext db, int id, МаркиАвтомобилей updatedMark) =>
{
    var mark = await db.МаркиАвтомобилейs.FindAsync(id);
    if (mark == null) return Results.NotFound();

    mark.Наименование = updatedMark.Наименование;
    mark.ТехническиеХарактеристики = updatedMark.ТехническиеХарактеристики;
    mark.Описание = updatedMark.Описание;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Удаление марки автомобиля
app.MapDelete("/api/МаркиАвтомобилей/{id}", async (ProkatContext db, int id) =>
{
    var mark = await db.МаркиАвтомобилейs.FindAsync(id);
    if (mark == null) return Results.NotFound();

    db.МаркиАвтомобилейs.Remove(mark);
    await db.SaveChangesAsync();
    return Results.Ok(mark);
});
// Получение списка всех сотрудников
app.MapGet("/api/Сотрудники", async (ProkatContext db) =>
{
    return Results.Ok(await db.Сотрудникиs.ToListAsync());
});

// Добавление нового сотрудника
app.MapPost("/api/Сотрудники", async (ProkatContext db, Сотрудники newEmployee) =>
{
    db.Сотрудникиs.Add(newEmployee);
    await db.SaveChangesAsync();
    return Results.Created($"/api/Сотрудники/{newEmployee.IdСотрудника}", newEmployee);
});

// Обновление данных сотрудника
app.MapPut("/api/Сотрудники/{id}", async (ProkatContext db, int id, Сотрудники updatedEmployee) =>
{
    var employee = await db.Сотрудникиs.FindAsync(id);
    if (employee == null) return Results.NotFound();

    // Обновление полей сотрудника
    employee.Фио = updatedEmployee.Фио ?? employee.Фио;
    employee.Возраст = updatedEmployee.Возраст ?? employee.Возраст;
    employee.Пол = updatedEmployee.Пол ?? employee.Пол;
    employee.Адрес = updatedEmployee.Адрес ?? employee.Адрес;
    employee.Телефон = updatedEmployee.Телефон ?? employee.Телефон;
    employee.ПаспортныеДанные = updatedEmployee.ПаспортныеДанные ?? employee.ПаспортныеДанные;
    employee.IdДолжности = updatedEmployee.IdДолжности ?? employee.IdДолжности;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Удаление сотрудника
app.MapDelete("/api/Сотрудники/{id}", async (ProkatContext db, int id) =>
{
    var employee = await db.Сотрудникиs.FindAsync(id);
    if (employee == null) return Results.NotFound();

    db.Сотрудникиs.Remove(employee);
    await db.SaveChangesAsync();
    return Results.Ok(employee);
});
// Получение списка всех записей проката
app.MapGet("/api/Прокат", async (ProkatContext db) =>
{
    return Results.Ok(await db.Прокатs.ToListAsync());
});

// Добавление новой записи проката
app.MapPost("/api/Прокат", async (ProkatContext db, Прокат newRental) =>
{
    db.Прокатs.Add(newRental);
    await db.SaveChangesAsync();
    return Results.Created($"/api/Прокат/{newRental.IdАвтомобиля}", newRental); // Используйте соответствующий идентификатор
});

// Обновление записи проката
app.MapPut("/api/Прокат/{id}", async (ProkatContext db, int id, Прокат updatedRental) =>
{
    var rental = await db.Прокатs.FindAsync(id);
    if (rental == null) return Results.NotFound();

    // Обновление полей записи проката
    rental.ДатаПроката = updatedRental.ДатаПроката ?? rental.ДатаПроката;
    rental.ПериодПроката = updatedRental.ПериодПроката ?? rental.ПериодПроката;
    rental.ДатаВозврата = updatedRental.ДатаВозврата ?? rental.ДатаВозврата;
    rental.IdАвтомобиля = updatedRental.IdАвтомобиля ?? rental.IdАвтомобиля;
    rental.IdКлиента = updatedRental.IdКлиента ?? rental.IdКлиента;
    rental.IdУслуги1 = updatedRental.IdУслуги1 ?? rental.IdУслуги1;
    rental.IdУслуги2 = updatedRental.IdУслуги2 ?? rental.IdУслуги2;
    rental.IdУслуги3 = updatedRental.IdУслуги3 ?? rental.IdУслуги3;
    rental.ЦенаПроката = updatedRental.ЦенаПроката ?? rental.ЦенаПроката;
    rental.ОтметкаОплате = updatedRental.ОтметкаОплате ?? rental.ОтметкаОплате;
    rental.IdСотрудника = updatedRental.IdСотрудника ?? rental.IdСотрудника;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Удаление записи проката
app.MapDelete("/api/Прокат/{id}", async (ProkatContext db, int id) =>
{
    var rental = await db.Прокатs.FindAsync(id);
    if (rental == null) return Results.NotFound();

    db.Прокатs.Remove(rental);
    await db.SaveChangesAsync();
    return Results.Ok(rental);
});
// Получение списка всех дополнительных услуг
app.MapGet("/api/ДополнительныеУслуги", async (ProkatContext db) =>
{
    return Results.Ok(await db.ДополнительныеУслугиs.ToListAsync());
});

// Добавление новой дополнительной услуги
app.MapPost("/api/ДополнительныеУслуги", async (ProkatContext db, ДополнительныеУслуги newService) =>
{
    db.ДополнительныеУслугиs.Add(newService);
    await db.SaveChangesAsync();
    return Results.Created($"/api/ДополнительныеУслуги/{newService.IdУслуги}", newService);
});

// Обновление дополнительной услуги
app.MapPut("/api/ДополнительныеУслуги/{id}", async (ProkatContext db, int id, ДополнительныеУслуги updatedService) =>
{
    var service = await db.ДополнительныеУслугиs.FindAsync(id);
    if (service == null) return Results.NotFound();

    // Обновление полей дополнительной услуги
    service.Наименование = updatedService.Наименование ?? service.Наименование;
    service.Описание = updatedService.Описание ?? service.Описание;
    service.Цена = updatedService.Цена ?? service.Цена;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Удаление дополнительной услуги
app.MapDelete("/api/ДополнительныеУслуги/{id}", async (ProkatContext db, int id) =>
{
    var service = await db.ДополнительныеУслугиs.FindAsync(id);
    if (service == null) return Results.NotFound();

    db.ДополнительныеУслугиs.Remove(service);
    await db.SaveChangesAsync();
    return Results.Ok(service);
});

app.MapGet("/api/ОтделКадров", async (ProkatContext db) =>
{
    return Results.Ok(await db.ОтделКадровs.ToListAsync());
});

// Добавление нового сотрудника в отдел кадров
app.MapPost("/api/ОтделКадров", async (ProkatContext db, ОтделКадров newHR) =>
{
    db.ОтделКадровs.Add(newHR);
    await db.SaveChangesAsync();
    return Results.Created($"/api/ОтделКадров/{newHR.Фио}", newHR); // Используйте `Фио` как ключ
});

// Обновление данных сотрудника отдела кадров
app.MapPut("/api/ОтделКадров/{fio}", async (ProkatContext db, string fio, ОтделКадров updatedHR) =>
{
    var hr = await db.ОтделКадровs.FindAsync(fio);
    if (hr == null) return Results.NotFound();

    // Обновление полей сотрудника отдела кадров
    hr.Фио = updatedHR.Фио ?? hr.Фио;
    hr.Наименование = updatedHR.Наименование ?? hr.Наименование;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Удаление сотрудника отдела кадров
app.MapDelete("/api/ОтделКадров/{fio}", async (ProkatContext db, string fio) =>
{
    var hr = await db.ОтделКадровs.FindAsync(fio);
    if (hr == null) return Results.NotFound();

    db.ОтделКадровs.Remove(hr);
    await db.SaveChangesAsync();
    return Results.Ok(hr);
});
// Получение списка всех заказов
app.MapGet("/api/Заказы", async (ProkatContext db) =>
{
    return Results.Ok(await db.Заказыs.ToListAsync());
});

// Добавление нового заказа
app.MapPost("/api/Заказы", async (ProkatContext db, Заказы newOrder) =>
{
    db.Заказыs.Add(newOrder);
    await db.SaveChangesAsync();
    return Results.Created($"/api/Заказы/{newOrder.IdЗаказа}", newOrder);
});

// Обновление заказа
app.MapPut("/api/Заказы/{id}", async (ProkatContext db, int id, Заказы updatedOrder) =>
{
    var order = await db.Заказыs.FindAsync(id);
    if (order == null) return Results.NotFound();

    // Обновление полей заказа
    order.IdКлиента = updatedOrder.IdКлиента;
    order.IdАвтомобиля = updatedOrder.IdАвтомобиля;
    order.ДатаЗаказа = updatedOrder.ДатаЗаказа;
    order.ВремяЗаказа = updatedOrder.ВремяЗаказа;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Удаление заказа
app.MapDelete("/api/Заказы/{id}", async (ProkatContext db, int id) =>
{
    var order = await db.Заказыs.FindAsync(id);
    if (order == null) return Results.NotFound();

    db.Заказыs.Remove(order);
    await db.SaveChangesAsync();
    return Results.Ok(order);
});
// Получение списка всех автомобилей в автопарке
app.MapGet("/api/Автопарк", async (ProkatContext db) =>
{
    return Results.Ok(await db.Автопаркs.ToListAsync());
});

// Добавление нового автомобиля в автопарк
app.MapPost("/api/Автопарк", async (ProkatContext db, Автопарк newVehicle) =>
{
    db.Автопаркs.Add(newVehicle);
    await db.SaveChangesAsync();
    return Results.Created($"/api/Автопарк/{newVehicle.ГосНомер}", newVehicle);
});

// Обновление данных автомобиля в автопарке
app.MapPut("/api/Автопарк/{gosNumber}", async (ProkatContext db, string gosNumber, Автопарк updatedVehicle) =>
{
    var vehicle = await db.Автопаркs.FindAsync(gosNumber);
    if (vehicle == null) return Results.NotFound();

    // Обновление полей автомобиля
    vehicle.ГосНомер = updatedVehicle.ГосНомер ?? vehicle.ГосНомер;
    vehicle.Фио = updatedVehicle.Фио ?? vehicle.Фио;
    vehicle.НомерКузова = updatedVehicle.НомерКузова ?? vehicle.НомерКузова;
    vehicle.НомерДвигателя = updatedVehicle.НомерДвигателя ?? vehicle.НомерДвигателя;
    vehicle.Год = updatedVehicle.Год ?? vehicle.Год;
    vehicle.Пробег = updatedVehicle.Пробег ?? vehicle.Пробег;
    vehicle.Цена = updatedVehicle.Цена ?? vehicle.Цена;
    vehicle.ДатаПоследнегоТо = updatedVehicle.ДатаПоследнегоТо ?? vehicle.ДатаПоследнегоТо;
    vehicle.Особенности = updatedVehicle.Особенности ?? vehicle.Особенности;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Удаление автомобиля из автопарка
app.MapDelete("/api/Автопарк/{gosNumber}", async (ProkatContext db, string gosNumber) =>
{
    var vehicle = await db.Автопаркs.FindAsync(gosNumber);
    if (vehicle == null) return Results.NotFound();

    db.Автопаркs.Remove(vehicle);
    await db.SaveChangesAsync();
    return Results.Ok(vehicle);
});
// Получение списка всех автомобилей в прокате
app.MapGet("/api/АвтомобилиВПрокате", async (ProkatContext db) =>
{
    return Results.Ok(await db.АвтомобилиВПрокатеs.ToListAsync());
});

// Добавление нового автомобиля в прокат
app.MapPost("/api/АвтомобилиВПрокате", async (ProkatContext db, АвтомобилиВПрокате newRentalCar) =>
{
    db.АвтомобилиВПрокатеs.Add(newRentalCar);
    await db.SaveChangesAsync();
    return Results.Created($"/api/АвтомобилиВПрокате/{newRentalCar.ГосНомер}", newRentalCar);
});

// Обновление данных автомобиля в прокате
app.MapPut("/api/АвтомобилиВПрокате/{gosNumber}", async (ProkatContext db, string gosNumber, АвтомобилиВПрокате updatedRentalCar) =>
{
    var rentalCar = await db.АвтомобилиВПрокатеs.FindAsync(gosNumber);
    if (rentalCar == null) return Results.NotFound();

    // Обновление полей автомобиля в прокате
    rentalCar.ДатаПроката = updatedRentalCar.ДатаПроката ?? rentalCar.ДатаПроката;
    rentalCar.Наименование = updatedRentalCar.Наименование ?? rentalCar.Наименование;
    rentalCar.ТехническиеХарактеристики = updatedRentalCar.ТехническиеХарактеристики ?? rentalCar.ТехническиеХарактеристики;
    rentalCar.ГосНомер = updatedRentalCar.ГосНомер ?? rentalCar.ГосНомер;
    rentalCar.Фио = updatedRentalCar.Фио ?? rentalCar.Фио;
    rentalCar.Услуги1 = updatedRentalCar.Услуги1 ?? rentalCar.Услуги1;
    rentalCar.Услуги2 = updatedRentalCar.Услуги2 ?? rentalCar.Услуги2;
    rentalCar.Услуги3 = updatedRentalCar.Услуги3 ?? rentalCar.Услуги3;
    rentalCar.СотрудникиФио = updatedRentalCar.СотрудникиФио ?? rentalCar.СотрудникиФио;
    rentalCar.Возраст = updatedRentalCar.Возраст ?? rentalCar.Возраст;
    rentalCar.Пол = updatedRentalCar.Пол ?? rentalCar.Пол;
    rentalCar.Телефон = updatedRentalCar.Телефон ?? rentalCar.Телефон;
    rentalCar.ПаспортныеДанные = updatedRentalCar.ПаспортныеДанные ?? rentalCar.ПаспортныеДанные;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Удаление автомобиля из проката
app.MapDelete("/api/АвтомобилиВПрокате/{gosNumber}", async (ProkatContext db, string gosNumber) =>
{
    var rentalCar = await db.АвтомобилиВПрокатеs.FindAsync(gosNumber);
    if (rentalCar == null) return Results.NotFound();

    db.АвтомобилиВПрокатеs.Remove(rentalCar);
    await db.SaveChangesAsync();
    return Results.Ok(rentalCar);
});
app.Run();
public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    const string KEY = "mysupersecret_secretsecretsecretkey!123"; // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}