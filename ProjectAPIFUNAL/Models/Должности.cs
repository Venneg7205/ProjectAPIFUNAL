using System;
using System.Collections.Generic;

namespace ProjectAPIFUNAL.Models;

public partial class Должности
{
    public int IdДолжности { get; set; }

    public string? Наименование { get; set; }

    public decimal? Зарплата { get; set; }

    public string? Обязанности { get; set; }

    public string? Требования { get; set; }
}
