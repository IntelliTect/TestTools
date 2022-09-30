using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data;

internal class ContextConstructionInfo
{
    public ConstructorInfo ConstructorInfo { get; set; }
    public Lazy<DbContextOptions> Lazy { get; set; }
    public Type DbContextType { get; set; }
}